using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Diploma_cs.Controls;

public sealed class BarChartView : SKCanvasView
{
    public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
        nameof(Values),
        typeof(IReadOnlyList<float>),
        typeof(BarChartView),
        defaultValue: Array.Empty<float>(),
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty LabelsProperty = BindableProperty.Create(
        nameof(Labels),
        typeof(IReadOnlyList<string>),
        typeof(BarChartView),
        defaultValue: Array.Empty<string>(),
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(BarChartView),
        defaultValue: string.Empty,
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty BarColorProperty = BindableProperty.Create(
        nameof(BarColor),
        typeof(Color),
        typeof(BarChartView),
        defaultValue: Color.FromArgb("#4692E3"),
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(
        nameof(LabelFontSize),
        typeof(float),
        typeof(BarChartView),
        defaultValue: 16f,
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize),
        typeof(float),
        typeof(BarChartView),
        defaultValue: 18f,
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty YAxisLabelFontSizeProperty = BindableProperty.Create(
        nameof(YAxisLabelFontSize),
        typeof(float),
        typeof(BarChartView),
        defaultValue: 14f,
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty YAxisStepCountProperty = BindableProperty.Create(
        nameof(YAxisStepCount),
        typeof(int),
        typeof(BarChartView),
        defaultValue: 4,
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public static readonly BindableProperty YAxisFormatProperty = BindableProperty.Create(
        nameof(YAxisFormat),
        typeof(string),
        typeof(BarChartView),
        defaultValue: "0",
        propertyChanged: (b, _, __) => ((BarChartView)b).InvalidateSurface());

    public IReadOnlyList<float> Values
    {
        get => (IReadOnlyList<float>)GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public IReadOnlyList<string> Labels
    {
        get => (IReadOnlyList<string>)GetValue(LabelsProperty);
        set => SetValue(LabelsProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Color BarColor
    {
        get => (Color)GetValue(BarColorProperty);
        set => SetValue(BarColorProperty, value);
    }

    public float LabelFontSize
    {
        get => (float)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    public float TitleFontSize
    {
        get => (float)GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }

    public float YAxisLabelFontSize
    {
        get => (float)GetValue(YAxisLabelFontSizeProperty);
        set => SetValue(YAxisLabelFontSizeProperty, value);
    }

    public int YAxisStepCount
    {
        get => (int)GetValue(YAxisStepCountProperty);
        set => SetValue(YAxisStepCountProperty, value);
    }

    public string YAxisFormat
    {
        get => (string)GetValue(YAxisFormatProperty);
        set => SetValue(YAxisFormatProperty, value);
    }

    public BarChartView()
    {
        EnableTouchEvents = false;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        var values = Values ?? Array.Empty<float>();
        var labels = Labels ?? Array.Empty<string>();

        const float pad = 18f;

        using var labelPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 180),
            IsAntialias = true,
            TextSize = LabelFontSize
        };

        using var titlePaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 220),
            IsAntialias = true,
            TextSize = TitleFontSize,
            FakeBoldText = true
        };

        using var yAxisLabelPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 160),
            IsAntialias = true,
            TextSize = YAxisLabelFontSize
        };

        using var gridPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 20),
            IsAntialias = true,
            StrokeWidth = 1
        };

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(0, 0, 0, 60),
            IsAntialias = true,
            StrokeWidth = 2
        };

        using var barPaint = new SKPaint
        {
            Color = BarColor.ToSKColor().WithAlpha(230),
            IsAntialias = true
        };

        float titleArea = string.IsNullOrWhiteSpace(Title) ? 0f : Math.Max(TitleFontSize + 6f, 20f);
        float xLabelArea = Math.Max(LabelFontSize + 10f, 24f);

        float maxValue = values.Count == 0 ? 0f : values.Max();
        float yMax = Math.Max(1f, maxValue);
        int steps = Math.Clamp(YAxisStepCount, 2, 8);

        string maxLabel = yMax.ToString(YAxisFormat);
        float yLabelWidth = yAxisLabelPaint.MeasureText(maxLabel) + 10f;

        var chartRect = new SKRect(
            pad + yLabelWidth,
            pad + titleArea,
            info.Width - pad,
            info.Height - pad - xLabelArea);

        if (!string.IsNullOrWhiteSpace(Title))
            canvas.DrawText(Title, chartRect.Left, pad + TitleFontSize, titlePaint);

        if (values.Count == 0)
        {
            var msg = "Brak danych";
            var w = titlePaint.MeasureText(msg);
            canvas.DrawText(msg, (info.Width - w) / 2f, info.Height / 2f, titlePaint);
            return;
        }

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            float y = chartRect.Bottom - (t * chartRect.Height);

            canvas.DrawLine(chartRect.Left, y, chartRect.Right, y, gridPaint);

            float v = t * yMax;
            string text = v.ToString(YAxisFormat);
            float tw = yAxisLabelPaint.MeasureText(text);

            canvas.DrawText(
                text,
                chartRect.Left - 8f - tw,
                y + (YAxisLabelFontSize * 0.35f),
                yAxisLabelPaint);
        }

        canvas.DrawLine(chartRect.Left, chartRect.Top, chartRect.Left, chartRect.Bottom, axisPaint);
        canvas.DrawLine(chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom, axisPaint);

        int n = values.Count;
        float slotW = chartRect.Width / n;
        float barW = slotW * 0.55f;

        for (int i = 0; i < n; i++)
        {
            float v = Math.Max(0f, values[i]);
            float h = (v / yMax) * chartRect.Height;
            float cx = chartRect.Left + (i + 0.5f) * slotW;

            var bar = new SKRect(cx - barW / 2f, chartRect.Bottom - h, cx + barW / 2f, chartRect.Bottom);
            canvas.DrawRoundRect(bar, 6, 6, barPaint);

            if (i < labels.Count)
            {
                var t = labels[i];
                var tw = labelPaint.MeasureText(t);
                canvas.DrawText(t, cx - tw / 2f, info.Height - pad, labelPaint);
            }
        }
    }
}
