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
                Style.Cbo => CBOStyle,
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
        };

        private static PhysicalStyle RoseStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = new SKColor(194, 30, 86),
            Back = new SKColor(248, 242, 243),
            Fore = new SKColor(75, 9, 23),
            Borders = new SKColor(149, 112, 119),
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
            PathToLeftSideImage = "./StylesImages/printstream.jpg",
            PathToRightSideImage = "./StylesImages/printstream.jpg"
        };

        private static PhysicalStyle CyberpunkStyle => new PhysicalStyle
        {
            HeadFore = new SKColor(216, 243, 249),
            HeadBack = new SKColor(75, 26, 115),
            Back = new SKColor(137, 94, 207),
            Fore = new SKColor(216, 243, 249),
            Borders = new SKColor(216, 243, 249),
            PathToLeftSideImage = "./StylesImages/cyberpunkLeft.jpg",
            PathToRightSideImage = "./StylesImages/cyberpunkRight.jpg"
        };

        private static PhysicalStyle SpaceStyle => new PhysicalStyle
        {
            HeadFore = new SKColor(255, 251, 226),
            HeadBack = new SKColor(9, 9, 11),
            Back = new SKColor(28, 53, 78),
            Fore = new SKColor(255, 251, 226),
            Borders = new SKColor(236, 202, 170),
            PathToLeftSideImage = "./StylesImages/spaceLeft.jpg",
            PathToRightSideImage = "./StylesImages/spaceRight.jpg"
        };

        private static PhysicalStyle CBOStyle => new PhysicalStyle
        {
            HeadFore = SKColors.White,
            HeadBack = new SKColor(223, 29, 37),
            Back = SKColors.White,
            Fore = SKColors.Black,
            Borders = new SKColor(53, 104, 161),
            PathToLeftSideImage = "./StylesImages/trikolor.jpg",
            PathToRightSideImage = "./StylesImages/cBo.png"
        };
    }
}
