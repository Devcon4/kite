using Kite.Domain;

namespace Kite.Mappers;
public static partial class Mapper {
  public static SettingModel SettingOptionsToSettingModel(SettingOptions settings) => new SettingModel(settings.AppName, settings.GroupOrder);
}