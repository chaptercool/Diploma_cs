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
        private int currentMonth = DateTime.Now.Month;
        private int currentYear = DateTime.Now.Year;
        private Dictionary<DateTime, DayDetailInfo> _dayDetailCache = new();

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
            DashedSeparator.Drawable = new DashedLineDrawable();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GenerateCalendarAsync();

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
                    var dayDetail = await _appDataService.GetDayDetailAsync(stat.Date);
                    _dayDetailCache[stat.Date.Date] = dayDetail;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading month data: {ex.Message}");
            }
        }

        private async Task AddDayCellAsync(int row, int col, int dayNumber, DateTime cellDate)
        {
            Border dayBorder = new Border
            {
                HeightRequest = 38,
                WidthRequest = 38,
                StrokeShape = new RoundRectangle { CornerRadius = 100 },
                Stroke = cellDate.Date == DateTime.Now.Date ? Color.FromArgb("#949494") : Colors.Transparent,
                StrokeThickness = cellDate.Date == DateTime.Now.Date ? 2 : 0
            };

            if (_dayDetailCache.TryGetValue(cellDate.Date, out var dayDetail))
            {
                dayBorder.Background = new SolidColorBrush(dayDetail.GetStatusColor());

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) => await ShowDayDetailsAsync(dayDetail);
                dayBorder.GestureRecognizers.Add(tapGesture);
            }
            else
            {
                dayBorder.Background = new SolidColorBrush(Color.FromArgb("#E6E8EB"));
            }

            Label dayLabel = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            if (_dayDetailCache.ContainsKey(cellDate.Date))
            {
                dayLabel.TextColor = Colors.White;
            }

            dayBorder.Content = dayLabel;
            calendarGrid.Add(dayBorder, col, row);
        }

        private async Task ShowDayDetailsAsync(DayDetailInfo dayDetail)
        {
            try
            {
                string message = $"Data: {dayDetail.Date:d}\n" +
                                $"Sesji palenia: {dayDetail.SessionsCount}\n" +
                                $"Zakupów paczek: {dayDetail.PacksCount}\n" +
                                $"Cel dzienny: {dayDetail.Target}\n" +
                                $"Status: {dayDetail.GetStatusText()}";

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