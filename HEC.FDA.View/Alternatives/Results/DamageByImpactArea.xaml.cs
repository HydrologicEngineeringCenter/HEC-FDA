using HEC.FDA.ViewModel.Alternatives.Results;
using System.Windows.Controls;

namespace HEC.FDA.View.Alternatives.Results
{
    /// <summary>
    /// Interaction logic for EADDamageByImpactArea.xaml
    /// </summary>
    public partial class EADDamageByImpactArea : UserControl
    {
        public EADDamageByImpactArea()
        {
            InitializeComponent();
        }

        private void FdaDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == nameof(IConsequenceByImpactAreaRowItem.Value))
                return;

            if (e.Column is DataGridTextColumn textColumn)
            {
                switch (e.PropertyName)
                {
                    case nameof(IConsequenceByImpactAreaRowItem.ImpactArea):
                        textColumn.Width = 110;
                        break;
                    default:
                        textColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                        break;
                }
                textColumn.MinWidth = 110;
            }
        }
    }
}
