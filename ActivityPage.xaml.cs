using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System.Diagnostics;
using Diploma_cs.Data.Services;
using Diploma_cs.Services;
using Diploma_cs.Models;

namespace Diploma_cs
{
    public partial class ActivityPage : ContentPage
    {
        private readonly AppDataService _appDataService;
        private readonly UiStatisticsService _uiStatisticsService;
        private int currentMonth = DateTime.Now.Month;
        private int currentYear = DateTime.Now.Year;

        // Cache is keyed strictly by date-only (DateTime at midnight, Kind=Unspecified)
        private readonly Dictionary<DateTime, DayDetailInfo> _dayDetailCache = new();

        public class DashedLineDrawable : IDrawable
        {
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.StrokeColor = Color.FromArgb("#DBD9D7");
                canvas.StrokeSize = 3;
                canvas.StrokeDashPattern = new float[] { 6, 4 };
                float y = dirtyRect.Height / 2;
                canvas.DrawLine(0, y, dirtyRect.Width, y);
            }
        }

        public ActivityPage()
        {
            InitializeComponent();
            _appDataService = ServiceHelper.GetService<AppDataService>();
            _uiStatisticsService = ServiceHelper.GetService<UiStatisticsService>();
            DashedSeparator.Drawable = new DashedLineDrawable();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GenerateCalendarAsync();

            try
            {
                var stats = await _uiStatisticsService.GetLast7DaysAsync();
                Last7DaysStats.Stats = stats;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load 7-day stats: {ex.Message}");
            }
        }

        private static DateTime ToDateKey(DateTime dt) => DateTime.SpecifyKind(dt.Date, DateTimeKind.Unspecified);

        private static Color StatusToColor(DailyStats.DayStatus status)
        {
            return status switch
            {
                DailyStats.DayStatus.Ok => Color.FromArgb("#9EC1FB"),
                DailyStats.DayStatus.Target => Color.FromArgb("#4692E3"),
                DailyStats.DayStatus.Exceeded => Color.FromArgb("#CC1714"),
                _ => Color.FromArgb("#E6E8EB")
            };
        }

        private async Task GenerateCalendarAsync()
        {
            try
            {
                ClearCalendarCells();

                DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
                int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
                int firstDayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

                monthYearLabel.Text = firstDayOfMonth.ToString("MMMM yyyy");

                await LoadMonthDataAsync();

                int day = 1;
                for (int row = 1; row < 7 && day <= daysInMonth; row++)
                {
                    for (int col = 0; col < 7 && day <= daysInMonth; col++)
                    {
                        if (row == 1 && col < firstDayOfWeek)
                        {
                            continue;
                        }

                        DateTime cellDate = new DateTime(currentYear, currentMonth, day);
                        await AddDayCellAsync(row, col, day, cellDate);
                        day++;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ kalendarz: {ex.Message}", "OK");
            }
        }

        private async Task LoadMonthDataAsync()
        {
            try
            {
                DateTime firstDay = new DateTime(currentYear, currentMonth, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

                var monthStats = await _appDataService.GetDailyStatsRangeAsync(firstDay, lastDay);

                _dayDetailCache.Clear();
                foreach (var stat in monthStats)
                {
                    var key = ToDateKey(stat.Date);
                    var dayDetail = await _appDataService.GetDayDetailAsync(key);
                    _dayDetailCache[key] = dayDetail;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading month data: {ex.Message}");
            }
        }

        private async Task<Color> GetDayColorAsync(DateTime cellKey)
        {
            if (_dayDetailCache.TryGetValue(cellKey, out var cached))
                return cached.GetStatusColor();

            var detail = await _appDataService.GetDayDetailAsync(cellKey);
            _dayDetailCache[cellKey] = detail;

            if (detail.SessionsCount == 0 && detail.PacksCount == 0 && detail.Target == 0)
                return Color.FromArgb("#E6E8EB");

            return detail.GetStatusColor();
        }

        private async Task AddDayCellAsync(int row, int col, int dayNumber, DateTime cellDate)
        {
            var cellKey = ToDateKey(cellDate);

            Border dayBorder = new Border
            {
                HeightRequest = 38,
                WidthRequest = 38,
                StrokeShape = new RoundRectangle { CornerRadius = 100 },
                Stroke = cellKey == ToDateKey(DateTime.Now) ? Color.FromArgb("#949494") : Colors.Transparent,
                StrokeThickness = cellKey == ToDateKey(DateTime.Now) ? 2 : 0
            };

            var bg = await GetDayColorAsync(cellKey);
            dayBorder.Background = new SolidColorBrush(bg);

            var hasData = bg != Color.FromArgb("#E6E8EB");

            var contentGrid = new Grid();

            Label dayLabel = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = hasData ? Colors.White : Colors.Black
            };

            var tapOverlay = new BoxView
            {
                BackgroundColor = Colors.Transparent,
                InputTransparent = false
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await ShowDayDetailsAsync(cellKey);
            tapOverlay.GestureRecognizers.Add(tapGesture);

            contentGrid.Children.Add(dayLabel);
            contentGrid.Children.Add(tapOverlay);

            dayBorder.Content = contentGrid;
            calendarGrid.Add(dayBorder, col, row);
        }

        private async Task ShowDayDetailsAsync(DateTime dayKey)
        {
            try
            {
                var key = ToDateKey(dayKey);

                if (!_dayDetailCache.TryGetValue(key, out var dayDetail))
                {
                    dayDetail = await _appDataService.GetDayDetailAsync(key);
                    _dayDetailCache[key] = dayDetail;
                }

                string message = $"Data: {key:d}\n" +
                                $"Zarejestrowanych sesji: {dayDetail.SessionsCount}\n" +
                                $"Zakupiono paczek: {dayDetail.PacksCount}\n" +
                                $"Cel: {dayDetail.Target}\n" +
                                $"Tzymanie siê celu: {dayDetail.GetStatusText()}";

                await DisplayAlert("Szczegó³y dnia", message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("B³¹d", $"Nie uda³o siê za³adowaæ szczegó³y dnia: {ex.Message}", "OK");
            }
        }

        private void ClearCalendarCells()
        {
            for (int row = 1; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    var existingElement = calendarGrid.Children.FirstOrDefault(x =>
                        calendarGrid.GetRow(x) == row && calendarGrid.GetColumn(x) == col);

                    if (existingElement != null)
                    {
                        calendarGrid.Children.Remove(existingElement);
                    }
                }
            }
        }

        private async void PreviousMonthClicked(object sender, EventArgs e)
        {
            if (currentMonth == 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else
            {
                currentMonth--;
            }

            await GenerateCalendarAsync();
            UpdateNavigationArrows();
        }

        private async void NextMonthClicked(object sender, EventArgs e)
        {
            if (currentMonth == 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            else
            {
                currentMonth++;
            }

            await GenerateCalendarAsync();
            UpdateNavigationArrows();
        }

        private void UpdateNavigationArrows()
        {
            previousMonthButton.Source = "arrowleftactive.png";
            nextMonthButton.Source = "arrowrightactive.png";
        }
    }
}