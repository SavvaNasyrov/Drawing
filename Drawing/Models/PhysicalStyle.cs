using DrawingService;
using SkiaSharp;

namespace Drawing.Models
{
    public record PhysicalStyle
    {
        public required SKColor HeadBack { get; init; }

        public required SKColor HeadFore { get; init; }

        public required SKColor Back {  get; init; }

        public required SKColor Fore { get; init; }

        public required SKColor Borders { get; init; }

        public required SKColor Diff { get; init; }

        public string? PathToLeftSideImage { get; init; }

        public string? PathToRightSideImage { get; init; }

        public static PhysicalStyle ToPhisycal(Style style)
        {
            return style switch
            {
                Style.Classic => ClassicStyle,
                Style.Rose => RoseStyle,
                Style.PrintStream => PrintstreamStyle,
                Style.Cyberpunk => CyberpunkStyle,
                Style.Space => SpaceStyle,
                Style.RyanGosling => RyanGoslingStyle,
                Style.Capybara => CapyStyle,
                _ => throw new NotImplementedException(),
            };
        }

        private static PhysicalStyle ClassicStyle => new PhysicalStyle 
        { 
            HeadFore = SKColors.White ,
            HeadBack = new SKColor(0, 128, 0) ,
            Back = SKColors.White ,
            Fore = SKColors.Black ,
            Borders = SKColors.Gray ,
            Diff = new SKColor(244, 150, 34)
        };

        private static PhysicalStyle RoseStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = new SKColor(194, 30, 86),
            Back = new SKColor(248, 242, 243),
            Fore = new SKColor(75, 9, 23),
            Borders = new SKColor(149, 112, 119),
            Diff = new SKColor(244, 150, 34),
            PathToLeftSideImage = "./StylesImages/roses.jpg",
            PathToRightSideImage = "./StylesImages/roses.jpg"
        };

        private static PhysicalStyle PrintstreamStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = SKColors.Black,
            Back = SKColors.White,
            Fore = SKColors.Black,
            Borders = new SKColor(153, 174, 190),
            Diff = new SKColor(244, 150, 34),
            PathToLeftSideImage = "./StylesImages/printstream.jpg",
            PathToRightSideImage = "./StylesImages/printstream.jpg"
        };

        private static PhysicalStyle CyberpunkStyle => new PhysicalStyle
        {
            HeadFore = new SKColor(253, 246, 33),
            HeadBack = SKColors.Black,
            Back = SKColors.White,
            Fore = SKColors.Black,
            Borders = new SKColor(45, 111, 122),
            Diff = new SKColor(0, 97, 112),
            PathToLeftSideImage = "./StylesImages/cyberpunk.png",
            PathToRightSideImage = "./StylesImages/cyberpunkRight.png"
        };

        private static PhysicalStyle SpaceStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = SKColors.Black,
            Back = new SKColor(34, 46, 58),
            Fore = SKColors.White,
            Borders = new SKColor(121, 137, 153),
            Diff = new SKColor(253, 194, 111),
            PathToLeftSideImage = "./StylesImages/spaceLeft.jpg",
            PathToRightSideImage = "./StylesImages/space.jpg"
        };

        private static PhysicalStyle RyanGoslingStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = new SKColor(94, 121, 114),
            Back = new SKColor(234, 251, 245),
            Fore = SKColors.Black,
            Borders = new SKColor(45, 68, 69),
            Diff = new SKColor(249, 145, 24),
            PathToLeftSideImage = "./StylesImages/bladeruiner.jpg",
            PathToRightSideImage = "./StylesImages/bladeruinerRight.jpg"
        };

        private static PhysicalStyle CapyStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = new SKColor(255, 170, 0),
            Back = SKColors.White,
            Fore = SKColors.Black,
            Borders = new SKColor(255, 202, 0),
            Diff = new SKColor(15, 128, 64),
            PathToLeftSideImage = "./StylesImages/capy.png",
            PathToRightSideImage = "./StylesImages/capy.png"
        };
    }
}
