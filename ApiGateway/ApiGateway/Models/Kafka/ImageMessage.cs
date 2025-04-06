namespace ApiGateway.Models.Kafka
{
    public class ImageMessage
    {
        public string Image { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool PreserveAspectRatio { get; set; }

        public double Angle { get; set; }

        public string Format { get; set; }
    }
}
