namespace HEC.FDA.ViewModel.Inventory.DamageCategory
{
    public class DamageCategoryFactory
    {
        public static IDamageCategory Factory(string name)
        {
            return new DamageCategory(name);
        }
    }
}
