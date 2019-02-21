using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Promotions
{
    public class PromotionRewardJsonConverter : JsonConverter
    {
        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(PromotionReward).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            if (typeof(PromotionReward).IsAssignableFrom(objectType))
            {
                var rewardType = objectType.Name;
                var pt = obj["Id"] ?? obj["id"];
                if (pt != null)
                {
                    rewardType = pt.Value<string>();
                }
                retVal = AbstractTypeFactory<PromotionReward>.TryCreateInstance(rewardType);
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown reward: " + rewardType);
                }

            }
            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }
    }
}
