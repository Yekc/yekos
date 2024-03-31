using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;

namespace Yek.Resources
{
    public static class ResourceLoader
    {
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.ka8x16thin-1.psf")] public static byte[] thin;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.cp850-8x8.psf")] public static byte[] tiny;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.lat0-sun16.psf")] public static byte[] sun;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.CaviarDreams_Bold.psf")] public static byte[] modern_f;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.gr737-9x16-medieval.psf")] public static byte[] medieval;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-phaisarn.f16.psf")] public static byte[] tisaisarn;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptconsl.f16.psf")] public static byte[] tisconsl;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptcufont.f20.psf")] public static byte[] tiscufont;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptkmfont.f24.psf")] public static byte[] tiskmfont;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptlight.f16.psf")] public static byte[] tislight;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptramafo.f20.psf")] public static byte[] tisramafo;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.tis-ptsmall.f16.psf")] public static byte[] tissmall;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.test.bmp")] public static byte[] test;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter1.bmp")] public static byte[] hampter1;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter2.bmp")] public static byte[] hampter2;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter3.bmp")] public static byte[] hampter3;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter4.bmp")] public static byte[] hampter4;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter5.bmp")] public static byte[] hampter5;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.hampter6.bmp")] public static byte[] hampter6;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.modern.bmp")] public static byte[] modern;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.map1.bmp")] public static byte[] map1;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.map2.bmp")] public static byte[] map2;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.arrow.bmp")] public static byte[] arrow;
        [ManifestResourceStream(ResourceName = "yek-dev.Resources.rat.bmp")] public static byte[] rat;

        public static PCScreenFont FontThin = PCScreenFont.LoadFont(thin);
        public static PCScreenFont FontTiny = PCScreenFont.LoadFont(tiny);
        public static PCScreenFont FontSun = PCScreenFont.LoadFont(sun);
        public static PCScreenFont FontModern = PCScreenFont.LoadFont(modern_f);
        public static PCScreenFont FontMedieval = PCScreenFont.LoadFont(medieval);
        public static PCScreenFont FontTisAisarn = PCScreenFont.LoadFont(tisaisarn);
        public static PCScreenFont FontTisConsl = PCScreenFont.LoadFont(tisconsl);
        public static PCScreenFont FontTisCufont = PCScreenFont.LoadFont(tiscufont);
        public static PCScreenFont FontTisKmfont = PCScreenFont.LoadFont(tiskmfont);
        public static PCScreenFont FontTisLight = PCScreenFont.LoadFont(tislight);
        public static PCScreenFont FontTisRamafo = PCScreenFont.LoadFont(tisramafo);
        public static PCScreenFont FontTisSmall = PCScreenFont.LoadFont(tissmall);
        public static Bitmap Test;
        public static Bitmap Hampter1;
        public static Bitmap Hampter2;
        public static Bitmap Hampter3;
        public static Bitmap Hampter4;
        public static Bitmap Hampter5;
        public static Bitmap Hampter6;
        public static Bitmap Modern;
        public static Bitmap Map1;
        public static Bitmap Map2;
        public static Bitmap Arrow;
        public static Bitmap Rat;

        public static void Load()
        {
            Test = new Bitmap(test);
            Hampter1 = new Bitmap(hampter1);
            Hampter2 = new Bitmap(hampter2);
            Hampter3 = new Bitmap(hampter3);
            Hampter4 = new Bitmap(hampter4);
            Hampter5 = new Bitmap(hampter5);
            Hampter6 = new Bitmap(hampter6);
            Modern = new Bitmap(modern);
            Map1 = new Bitmap(map1);
            Map2 = new Bitmap(map2);
            Arrow = new Bitmap(arrow);
            Rat = new Bitmap(rat);
        }
    }
}
