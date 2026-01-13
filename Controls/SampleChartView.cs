using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Diploma_cs.Controls;

public sealed class SampleChartView : SKCanvasView
{
    public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
        nameof(Values),
        typeof(IReadOnlyList<float>),
        typeof(SampleChartView),
        defaultValue: Array.Empty<float>(),
        propertyChanged: (_, __, ___) => ((SampleChartView)_).InvalidateSurface());

    public static readonly BindableProperty LabelsProperty = BindableProperty.Create(
        nameof(Labels),
        typeof(IReadOnlyList<string>),
        typeof(SampleChartView),
        defaultValue: Array.Empty<string>(),
        propertyChanged: (_, __, ___) => ((SampleChartView)_).InvalidateSurface());

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

    public SampleChartView()
    {
        EnableTouchEvents = false;
        IgnorePixelScaling = false;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        var values = Values ?? Array.Empty<float>();
        var labels = Labels ?? Array.Empty<string>();

        if (values.Count == 0)
        {
            using var emptyPaint = new SKPaint
            {
                Color = new SKColor(255, 255, 255, 160),
                IsAntialias = true,
                TextSize = 14
            };

            var msg = "No data";
            var w = emptyPaint.MeasureText(msg);
            canvas.DrawText(msg, (info.Width - w) / 2f, info.Height / 2f, emptyPaint);
            return;
        }

        float max = Math.Max(1f, values.Max());

        float pad = 20f;
        float labelArea = 22f;
        var chartRect = new SKRect(pad, pad, info.Width - pad, info.Height - pad - labelArea);

        using var axisPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 40),
            IsAntialias = true,
            StrokeWidth = 2
        };

        using var gridPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 20),
            IsAntialias = true,
            StrokeWidth = 1
        };

        using var barPaint = new SKPaint
        {
            Color = new SKColor(70, 146, 227, 230),
            IsAntialias = true
        };

        using var linePaint = new SKPaint
        {
            Color = new SKColor(158, 193, 251, 230),
            IsAntialias = true,
            StrokeWidth = 3,
            Style = SKPaintStyle.Stroke
        };

        using var pointPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 220),
            IsAntialias = true
        };

        using var labelPaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 170),
            IsAntialias = true,
            TextSize = 12
        };

        // grid (3 horizontal lines)
        for (int i = 1; i <= 3; i++)
        {
            float y = chartRect.Top + (chartRect.Height * i / 4f);
            canvas.DrawLine(chartRect.Left, y, chartRect.Right, y, gridPaint);
        }

        // axes baseline
        canvas.DrawLine(chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom, axisPaint);

        int n = values.Count;
        float slotW = chartRect.Width / n;
        float barW = slotW * 0.55f;

        var path = new SKPath();

        for (int i = 0; i < n; i++)
        {
            float v = Math.Max(0, values[i]);
            float h = (v / max) * chartRect.Height;
            float cx = chartRect.Left + (i + 0.5f) * slotW;

            // bars
            var bar = new SKRect(cx - barW / 2f, chartRect.Bottom - h, cx + barW / 2f, chartRect.Bottom);
            canvas.DrawRoundRect(bar, 6, 6, barPaint);

            // line chart over bars
            float px = cx;
            float py = chartRect.Bottom - h;
            if (i == 0)
                path.MoveTo(px, py);
            else
                path.LineTo(px, py);

            canvas.DrawCircle(px, py, 4.5f, pointPaint);

            // labels
            if (i < labels.Count)
            {
                var t = labels[i];
                var tw = labelPaint.MeasureText(t);
                canvas.DrawText(t, px - tw / 2f, info.Height - pad, labelPaint);
            }
        }

        canvas.DrawPath(path, linePaint);

        // simple title
        using var titlePaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 200),
            IsAntialias = true,
            TextSize = 14,
            FakeBoldText = true
        };

        canvas.DrawText("Sample chart (SkiaSharp)", chartRect.Left, chartRect.Top - 4, titlePaint);
    }
}
