using StardewModdingAPI;

namespace OmniFarm
{
    public class MyAssetLoader : IAssetLoader
    {
        private IModHelper helper;

        public MyAssetLoader(IModHelper helper)
        {
            this.helper = helper;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Maps\FarmCave");
        }

        public T Load<T>(IAssetInfo asset)
        {
            return this.helper.Content.Load<T>(@"assets\FarmCave.tbin");
        }
    }
}