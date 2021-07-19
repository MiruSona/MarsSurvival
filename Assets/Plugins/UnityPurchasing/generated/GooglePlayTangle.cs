#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("nArANDuQEf6vnqC45S89G/AdSXyJweuA/nwDilQ2W2/l2UXYsiftnM0fkV64geBOXzVCWEg+neRSVsGvjH2+YOubG3YUNswdpw1/d1SznoRFSyVeFNjqsVah2qKNdgod4Xb58M9MQk19z0xHT89MTE3tE8YANsDvHP8ui5BYkBuv4w6il8vqqFByvJsB7m3Oe5uVX259qGeibA+VdWRx3e8PX9Jbqk1Tx5PmP8qAH9RqbnJL+fd0T7QhYpIqv2i5W5gW8K8MWG+g4TXSPiy099VldjwpAFcZT0LXs33PTG99QEtEZ8sFy7pATExMSE1OmEmge12n40RG3+XZ/eoVcNwIrhq7B08BqrqpYJ38HTG2M32k66cww95oN/9xHXvgCk9OTE1M");
        private static int[] order = new int[] { 6,13,2,10,10,13,10,12,8,13,11,11,13,13,14 };
        private static int key = 77;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
