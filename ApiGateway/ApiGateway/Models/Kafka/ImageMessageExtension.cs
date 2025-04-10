namespace ApiGateway.Models.Kafka
{
    public static class ImageMessageExstansion
    {
        public static void SetStep(this ImageMessage imageMessage)
        {
            imageMessage.IsNeedResize = imageMessage.Height > 0 || imageMessage.Width > 0;
            imageMessage.IsNeedRotation = imageMessage.Angle < 360 || imageMessage.Angle > -360;
        }
    }
}
