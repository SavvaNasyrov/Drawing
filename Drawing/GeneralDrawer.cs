﻿using Drawing.Models;
using DrawingService;
using SkiaSharp;

namespace Drawing
{
    public static class GeneralDrawer
    {
        public static void Draw(SortedSet<DrawRow> lessons, PhysicalStyle style, string pathToSave)
        {
            int indent = style.PathToLeftSideImage == null ? 0 : 40;
            const int ROW_HEIGHT = 70;
            const int FIRST_COLOMN_WIDTH = 200;
            const int SECOND_COLOMN_WIDTH = 840;
            const string HEADING_TEXT = "Уроки";

            int imageWidth = FIRST_COLOMN_WIDTH + SECOND_COLOMN_WIDTH;
            int imageHeight = ROW_HEIGHT * 8;

            var lessonsDict = lessons.ToDictionary(x => x.Number, y => y);
            var buffer = new List<DrawRow>(capacity: 7); 
            for (int i = 1; i < 8; i++)
            {
                bool has = lessonsDict.TryGetValue(i, out var row);
                if (has)
                    buffer.Add(row!);
                else
                    buffer.Add(new DrawRow { Number = i });
            }
            lessons = new SortedSet<DrawRow>(buffer);

            using var bitmap = new SKBitmap(
                width: imageWidth+ 2 * indent,
                height: imageHeight);
            using (var canvas = new SKCanvas(bitmap))
            {
                // Fill background.
                canvas.Clear(style.Back);

                // Draw sides images if exists. 
                if (style.PathToLeftSideImage != null)
                {
                    using var imgLeftBitmap = SKBitmap.Decode(style.PathToLeftSideImage);
                    using var styleLeftImage = SKImage.FromBitmap(imgLeftBitmap);
                    canvas.DrawImage(styleLeftImage, new SKPoint(0, 0));

                    using var imgRightBitmap = SKBitmap.Decode(style.PathToRightSideImage);
                    using var styleRightImage = SKImage.FromBitmap(imgRightBitmap);
                    canvas.DrawImage(styleRightImage, new SKPoint(imageWidth + indent, 0));
                }

                // Draw heading.
                using var headBackPaint = new SKPaint() { Color = style.HeadBack, Style = SKPaintStyle.Fill };
                canvas.DrawRect(indent, 0, imageWidth, ROW_HEIGHT, headBackPaint);

                using var headForePaint = new SKPaint() { Color = style.HeadFore, IsAntialias = true };
                using var headFont = new SKFont() { Size = 24, Embolden = true };

                DrawText(canvas, HEADING_TEXT, new SKRect(indent, 0, indent * 2 + imageWidth, ROW_HEIGHT), headForePaint, headFont);

                // Draw column border.
                using var borderPaint = new SKPaint() { Color = style.Borders };
                canvas.DrawLine(indent + FIRST_COLOMN_WIDTH, 0, indent + FIRST_COLOMN_WIDTH, ROW_HEIGHT * 8, borderPaint);

                // Draw rows.
                using var textPaint = new SKPaint() { Color = style.Fore, IsAntialias = true };
                using var diffPaint = new SKPaint() { Color = style.Diff, IsAntialias = true };
                using var dashedPaint = new SKPaint() { Color = style.Borders, PathEffect = SKPathEffect.CreateDash([10, 5], 0) };
                using var textFont = new SKFont() { Size = 24, Typeface = SKTypeface.FromFile("./Resources/Codec Pro/CodecPro-Regular.ttf") };

                SKPaint buff;

                int i = 1;
                foreach (var row in lessons)
                {
                    buff = row.First != null && row.First.IsDiff ? diffPaint : textPaint;
                    // Main data.
                    if (row.First != null && row.Second == null)
                    {
                        DrawText(
                            canvas,
                            LessonToString(row.First),
                            new SKRect(
                                left: indent + FIRST_COLOMN_WIDTH,
                                top: ROW_HEIGHT * i,
                                right: indent + imageWidth,
                                bottom: ROW_HEIGHT * (i + 1)),
                            buff,
                            textFont);
                    }
                    else if (row.First != null && row.Second != null)   
                    {
                        if (Math.Max(LessonToString(row.First).Length, LessonToString(row.Second).Length) <= 25)
                        {
                            DrawText(
                                canvas,
                                LessonToString(row.First),
                                new SKRect(
                                    left: indent + FIRST_COLOMN_WIDTH,
                                    top: ROW_HEIGHT * i,
                                    right: indent + imageWidth - SECOND_COLOMN_WIDTH / 2,
                                    bottom: ROW_HEIGHT * (i + 1)),
                                buff,
                                textFont);

                            DrawText(
                                canvas,
                                LessonToString(row.Second),
                                new SKRect(
                                    left: indent + FIRST_COLOMN_WIDTH + SECOND_COLOMN_WIDTH / 2,
                                    top: ROW_HEIGHT * i,
                                    right: indent + imageWidth,
                                    bottom: ROW_HEIGHT * (i + 1)),
                                buff,
                                textFont);
                        }
                        else
                        {
                            DrawText(
                                canvas,
                                LessonToString(row.First),
                                new SKRect(
                                    left: indent + FIRST_COLOMN_WIDTH,
                                    top: ROW_HEIGHT * i,
                                    right: indent + imageWidth,
                                    bottom: ROW_HEIGHT * i + ROW_HEIGHT / 2),
                                buff,
                                textFont);

                            DrawText(
                                canvas,
                                LessonToString(row.Second),
                                new SKRect(
                                    left: indent + FIRST_COLOMN_WIDTH,
                                    top: ROW_HEIGHT * i + ROW_HEIGHT / 2,
                                    right: indent + imageWidth,
                                    bottom: ROW_HEIGHT * (i + 1)),
                                buff,
                                textFont);

                            canvas.DrawLine(indent + FIRST_COLOMN_WIDTH, ROW_HEIGHT * i + ROW_HEIGHT / 2, indent + imageWidth, ROW_HEIGHT * i + ROW_HEIGHT / 2, dashedPaint);
                        }
                    }

                    // Line.
                    canvas.DrawLine(indent, ROW_HEIGHT * i, FIRST_COLOMN_WIDTH + SECOND_COLOMN_WIDTH + indent, ROW_HEIGHT * i, borderPaint);

                    // Number.
                    DrawText(
                            canvas,
                            row.First?.LessonNumberView ?? "",
                            new SKRect(
                                left: indent,
                                top: ROW_HEIGHT * i,
                                right: indent + FIRST_COLOMN_WIDTH,
                                bottom: ROW_HEIGHT * (i + 1)),
                            buff,
                            textFont);


                    i++;
                }
            }

            // Saving.
            var directory = Path.GetDirectoryName(pathToSave);
            // Проверяем, существует ли директория, и создаем её, если нет.
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(pathToSave);
            data.SaveTo(stream);
        }

        private static void DrawText(SKCanvas canvas, string text, SKRect area, SKPaint paint, SKFont font)
        {
            font.MeasureText(text, out var textBounds, paint);

            float textX = area.Left + (area.Width - textBounds.Width) / 2;
            float textY = area.Top + (area.Height - textBounds.Height) / 2 + textBounds.Height;

            canvas.DrawText(text, textX, textY, font, paint);
        }

        private static string LessonToString(Lesson lesson)
        {
            return $"{lesson.First}, {lesson.Second}, {lesson.Third}";
        }
    }
}
