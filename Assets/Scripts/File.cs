namespace FigmaLoader
{
    public class File
    {
        public Document Document;
    }

    public class Document
    {
        public Page[] Children;
    }

    public class Page
    {
        public Layer[] Children;
    }

    public class Layer
    {
        public FigmaType Type;
        public string Name;
        public Box AbsoluteBoundingBox;
        public Layer[] Children;
    }

    public enum FigmaType
    {
        Document,
        Canvas,
        Frame,
        Rectangle,
        Vector
    }

    public class Box
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
    }
}