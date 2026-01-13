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

        float pad = 20f;
        float labelArea = 22f;
        float titleArea = string.IsNullOrWhiteSpace(Title) ? 0f : 18f;

        var chartRect = new SKRect(pad, pad + titleArea, info.Width - pad, info.Height - pad - labelArea);

        using var gridPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 20),
            IsAntialias = true,
            StrokeWidth = 1
        };

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 40),
            IsAntialias = true,
            StrokeWidth = 2
        };

        using var barPaint = new SKPaint
        {
            Color = BarColor.ToSKColor().WithAlpha(230),
            IsAntialias = true
        };

        using var labelPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 170),
            IsAntialias = true,
            TextSize = 12
        };

        using var titlePaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 200),
            IsAntialias = true,
            TextSize = 14,
            FakeBoldText = true
        };

        if (!string.IsNullOrWhiteSpace(Title))
            canvas.DrawText(Title, chartRect.Left, pad + 12, titlePaint);

        if (values.Count == 0)
        {
            var msg = "No data";
            var w = titlePaint.MeasureText(msg);
            canvas.DrawText(msg, (info.Width - w) / 2f, info.Height / 2f, titlePaint);
            return;
        }

        float max = Math.Max(1f, values.Max());

        for (int i = 1; i <= 3; i++)
        {
            float y = chartRect.Top + (chartRect.Height * i / 4f);
            canvas.DrawLine(chartRect.Left, y, chartRect.Right, y, gridPaint);
        }

        canvas.DrawLine(chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom, axisPaint);

        int n = values.Count;
        float slotW = chartRect.Width / n;
        float barW = slotW * 0.55f;

        for (int i = 0; i < n; i++)
        {
            float v = Math.Max(0, values[i]);
            float h = (v / max) * chartRect.Height;
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
