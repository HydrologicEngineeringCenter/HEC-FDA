namespace HEC.FDA.ViewModel.Saving
{
    public class LoadingManager
    {

        public void Load()
        {

            //SavingFactory.GetRatingElementManager().Read()


            ////Elements.Clear();//clear out any existing ones from an existing study
            //if (Storage.Connection.Instance.IsConnectionNull) return;
            //Watershed.TerrainOwnerElement t = new Watershed.TerrainOwnerElement(null);


            ////AddElement(t);

            //ImpactArea.ImpactAreaOwnerElement i = new ImpactArea.ImpactAreaOwnerElement(this);
            //AddElement(i);

            //WaterSurfaceElevation.WaterSurfaceElevationOwnerElement wse = new WaterSurfaceElevation.WaterSurfaceElevationOwnerElement(this);


            //AddElement(wse);

            //FrequencyRelationships.FrequencyRelationshipsOwnerElement f = new FrequencyRelationships.FrequencyRelationshipsOwnerElement(this);
            //f.AddBaseElements();
            //AddElement(f);

            //FlowTransforms.FlowTransformsOwnerElement ft = new FlowTransforms.FlowTransformsOwnerElement(this);
            //ft.AddBaseElements();
            //AddElement(ft);

            ////InflowOutflow.InflowOutflowOwnerElement inout = new InflowOutflow.InflowOutflowOwnerElement(this);
            ////AddElement(inout);

            //StageTransforms.StageTransformsOwnerElement s = new StageTransforms.StageTransformsOwnerElement(this);
            //s.AddBaseElements();
            //AddElement(s);

            //Hydraulics.FloodPlainDataOwnerElement h = new Hydraulics.FloodPlainDataOwnerElement(this);
            ////this.AddElement(h);

            //GeoTech.LateralStructuresOwnerElement ls = new GeoTech.LateralStructuresOwnerElement(this);
            //ls.AddBaseElements();
            //AddElement(ls);

            ////GeoTech.LeveeFeatureOwnerElement l = new GeoTech.LeveeFeatureOwnerElement(this);
            ////this.AddElement(l);

            //Inventory.InventoryOwnerElement inv = new Inventory.InventoryOwnerElement(this);
            //inv.AddBaseElements();
            //AddElement(inv);

            //Conditions.ConditionsOwnerElement c = new Conditions.ConditionsOwnerElement(this);
            //AddElement(c);
        }




        public void AddChildrenFromTable()
        {
            //if (SavesToTable())
            //{
            //    DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(TableName);
            //    if (dtv != null)
            //    {
            //        //add an element based on a row element;
            //        if (!Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Open();
            //        for (int i = 0; i < dtv.NumberOfRows; i++)
            //        {
            //            AddElementFromRowData(dtv.GetRow(i));
            //        }
            //        if (Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Close();
            //    }
            //}
            //else
            //{
            //    foreach (BaseFdaElement ele in Elements)
            //    {
            //        if (ele is ParentElement)
            //        {
            //            ((ParentElement)ele).AddChildrenFromTable();
            //        }
            //    }
            //}

        }



    }
}