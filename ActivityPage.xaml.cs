using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace Diploma_cs
{
    public partial class ActivityPage : ContentPage
    {
        public enum DayStatus
        {
            Unavailable,
            NoRecord,
            Ok,
            Mid,
            Warning
        }

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

        private int currentMonth = 6;
        private int currentYear = 2025;

        public ActivityPage()
        {
            InitializeComponent();
            GenerateCalendar();
            DashedSeparator.Drawable = new DashedLineDrawable();

        }

        private void GenerateCalendar()
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

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);

            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            int firstDayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            monthYearLabel.Text = firstDayOfMonth.ToString("MMMM yyyy");

            Random random = new Random();

            int currentDay = random.Next(1, daysInMonth + 1);

            int day = 1;
            for (int row = 1; row < 7 && day <= daysInMonth; row++)
            {
                for (int col = 0; col < 7 && day <= daysInMonth; col++)
                {
                    if (row == 1 && col < firstDayOfWeek)
                    {
                        int prevMonthDay = DateTime.DaysInMonth(
                            currentMonth == 1 ? currentYear - 1 : currentYear,
                            currentMonth == 1 ? 12 : currentMonth - 1) -
                            (firstDayOfWeek - col - 1);

                        AddDayCell(row, col, prevMonthDay, DayStatus.Unavailable, false);
                        continue;
                    }

                    DayStatus status;
                    if (random.Next(10) == 0)
                    {
                        status = DayStatus.Unavailable;
                        if (day == currentDay)
                        {
                            currentDay = random.Next(1, daysInMonth + 1);
                        }
                    }
                    else
                    {
                        status = (DayStatus)random.Next(1, 5);
                    }

                    bool isCurrentDay = day == currentDay && status != DayStatus.Unavailable;
                    AddDayCell(row, col, day, status, isCurrentDay);
                    day++;
                }
            }
        }
        private void AddDayCell(int row, int col, int dayNumber, DayStatus status, bool isCurrentDay)
        {
            Border dayBorder = new Border
            {
                HeightRequest = 38,
                WidthRequest = 38,
                StrokeShape = new RoundRectangle { CornerRadius = 100 },
                Stroke = isCurrentDay ? Color.FromArgb("#949494") : Colors.Transparent,
                StrokeThickness = isCurrentDay ? 2 : 0
            };

            switch (status)
            {
                case DayStatus.Unavailable:
                    dayBorder.Background = Colors.Transparent;
                    break;
                case DayStatus.NoRecord:
                    dayBorder.Background = new SolidColorBrush(Color.FromArgb("#E6E8EB"));
                    break;
                case DayStatus.Ok:
                    dayBorder.Background = new SolidColorBrush(Color.FromArgb("#9EC1FB"));
                    break;
                case DayStatus.Mid:
                    dayBorder.Background = new SolidColorBrush(Color.FromArgb("#4692E3"));
                    break;
                case DayStatus.Warning:
                    dayBorder.Background = new SolidColorBrush(Color.FromArgb("#CC1714"));
                    break;
            }

            Label dayLabel = new Label
            {
                Text = dayNumber.ToString(),
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            if (status == DayStatus.Ok || status == DayStatus.Mid || status == DayStatus.Warning)
            {
                dayLabel.TextColor = Colors.White;
            }

            dayBorder.Content = dayLabel;
            calendarGrid.Add(dayBorder, col, row);
        }

        private void PreviousMonthClicked(object sender, EventArgs e)
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
            GenerateCalendar();
            UpdateNavigationArrows();
        }

        private void NextMonthClicked(object sender, EventArgs e)
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
            GenerateCalendar();
            UpdateNavigationArrows();
        }

        private void UpdateNavigationArrows()
        {
            previousMonthButton.Source = "arrowleftactive.png";
            nextMonthButton.Source = "arrowrightactive.png";
        }
    }
}