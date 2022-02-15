using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.ObjectModel;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// Note that there are only 5 of these in the occtype editor. There is NOT 5 per occtype.
    /// That means that these need to get updated with the new values everytime the occtype changes.
    /// </summary>
    public class ValueUncertaintyVM:BaseViewModel
    {
        public event EventHandler WasModified;

        #region fields
        private IDistribution _ValueUncertainty;
        private IValueUncertainty _CurrentVM;
        private NormalControlVM _NormalControlVM;
        private TriangularControlVM _TriangularControlVM;
        private UniformControlVM _UniformControlVM;
        private ValueUncertaintyType _valueUncertaintyType;

        #endregion

        #region properties
        public ValueUncertaintyType ValueUncertaintyType
        {
            get { return _valueUncertaintyType; }
            set 
            { 
                _valueUncertaintyType = value; 
                NotifyPropertyChanged();
            }
        }

        public IValueUncertainty CurrentVM
        {
            get { return _CurrentVM; }
            set { _CurrentVM = value; NotifyPropertyChanged(); }
        }


        public ObservableCollection<IDistributionEnum> UncertaintyTypes
        {
            get;
            set;
        }

        public IDistribution ValueUncertainty
        {
            get
            {
                return _ValueUncertainty;
            }
            set
            {
                _ValueUncertainty = value;
                SelectedDistributionTypeChanged();
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region constructors
        public ValueUncertaintyVM(IDistribution valueUncertaintyOrdinate, ValueUncertaintyType valueUncertaintyType, String labelString)
        {
            ValueUncertaintyType = ValueUncertaintyType;
            //create the options for the combobox
            UncertaintyTypes = new ObservableCollection<IDistributionEnum>()
            {
                IDistributionEnum.Deterministic,
                IDistributionEnum.Normal,
                IDistributionEnum.Triangular,
                IDistributionEnum.Uniform
            };

            //create the vm's for the individual distribution types

            //set what values you can, then set some defaults for the other dist types?
            CreateDistributionControls(valueUncertaintyOrdinate, labelString);

            ValueUncertainty = valueUncertaintyOrdinate;

            //set the current vm to be of the selected type
            SelectedDistributionTypeChanged();
        }

        #endregion

        /// <summary>
        /// The selected type of ordinate has changed. This method will convert the current ordinate
        /// into the new type using whatever values it can for the new one.
        /// </summary>
        /// <param name="newValue"></param>
        public void SelectionChanged(Object newValue)
        {
            if(newValue is IDistributionEnum)
            {
                IDistributionEnum newType = (IDistributionEnum)newValue;
                //if newtype is the same as current then do nothing
                if (newType != _ValueUncertainty.Type)
                {
                    WasModified(this, new EventArgs());
                    //it is possible that the current state of this control can't make an IOrdinate.
                    //If that is the case, then don't try to pass values into another dist type.
                    try 
                    { 
                        //the constant option doesn't have anything, so it can't make an IOrdinate
                        //if(_ValueUncertainty.Type != IDistributionEnum.Deterministic)
                        //{
                        //    _ValueUncertainty = _CurrentVM.CreateOrdinate();
                        //    IDistribution newOrdinate = IOrdinateTranslator.TranslateValuesBetweenDistributionTypes(_ValueUncertainty, newType);
                        //    ValueUncertainty = newOrdinate;
                        //}
                        //else
                        //{
                            //create a default IOrdinate of the new type
                            ValueUncertainty = CreateDefaultIOdinate(newType);
                        //}
                    
                    }
                    catch(Exception ex)
                    {
                        //create a default IOrdinate of the new type
                        IDistribution newOrdinate = CreateDefaultIOdinate(newType);
                        ValueUncertainty = newOrdinate;
                    }

                }

            }

            //{
            //    switch((IOrdinateEnum)newValue)
            //    {
            //        case IOrdinateEnum.Constant:
            //            {

            //                break;
            //            }
            //        case IOrdinateEnum.Normal:
            //            {

            //                break;
            //            }
            //        case IOrdinateEnum.Triangular:
            //            {

            //                break;
            //            }
            //        case IOrdinateEnum.Uniform:
            //            {

            //                break;
            //            }
            //    }
            //}
        }

        private IDistribution CreateDefaultIOdinate(IDistributionEnum type)
        {
            switch(type)
            {
                case IDistributionEnum.Deterministic:
                    {
                        return new Deterministic(0);
                    }
                case IDistributionEnum.Normal:
                    {
                        return new Normal(0, 0);
                    }
                case IDistributionEnum.Triangular:
                    {
                        return new Triangular(0,0, 0);

                    }
                case IDistributionEnum.Uniform:
                    {
                        return new Uniform(0, 0);

                    }
            }
            return null;
        }

        /// <summary>
        /// Creates all the distribution control view models. The occtype should
        /// have a value for each of the assets. I set those and then create default
        /// data for the other types so that when the user makes a different selection
        /// in the combobox, a view model with default values is displayed
        /// </summary>
        /// <param name="ordinate"></param>
        private void CreateDistributionControls(IDistribution ordinate, string labelString)
        {
            IDistributionEnum ordType = ordinate.Type;
            //create constant option
            if(ordType == IDistributionEnum.Deterministic)
            {
                //todo: i don't really know how to handle constant right now
                ControlWasModified(this, new EventArgs());
            }

            //create normal option
            double normalMean = 0;
            double normalStDev = 0;
            if(ordType == IDistributionEnum.Normal)
            {
                 normalMean = ((Normal)ordinate).Mean;
                normalStDev = ((Normal)ordinate).StandardDeviation;
            }
            _NormalControlVM = new NormalControlVM(normalMean, normalStDev, labelString);
            _NormalControlVM.WasModified += ControlWasModified;

            //create the triangular option
            double triMostLikely = 1;
            double triMin = 0;
            double triMax = 2;
            if(ordType == IDistributionEnum.Triangular)
            {
                triMostLikely = ((Triangular)ordinate).MostLikely;
                triMin = ((Triangular)ordinate).Min;
                triMax = ((Triangular)ordinate).Max;
            }
            _TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax, labelString);
            _TriangularControlVM.WasModified += ControlWasModified;

            //create the uniform option
            double uniMin = 0;
            double uniMax = 1;
            if(ordType == IDistributionEnum.Uniform)
            {
                uniMin = ((Uniform)ordinate).Min;
                uniMax = ((Uniform)ordinate).Max;
            }
            _UniformControlVM = new UniformControlVM(uniMin, uniMax, labelString);
            _UniformControlVM.WasModified += ControlWasModified;
        }

        private void ControlWasModified(object sender, EventArgs e)
        {
            WasModified?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Grabs the appropriate values from the ordinate and sets those values on the view model that will be displayed
        /// in the UI. Basically it makes sure that the correct values are in the text boxes.
        /// </summary>
        /// <param name="ordinate"></param>
        //private void UpdateDistributionValues(IOrdinate ordinate)
        //{
        //    IOrdinateEnum ordType = ordinate.Type;
        //    //create constant option
        //    if (ordType == IOrdinateEnum.Constant)
        //    {
        //        //todo: i don't really know how to handle constant right now
        //    }

        //    //create normal option
        //    double normalMean = 0;
        //    double normalStDev = 0;
        //    if (ordType == IOrdinateEnum.Normal)
        //    {
        //        normalMean = ((IDistributedOrdinate)ordinate).Mean;
        //        normalStDev = ((IDistributedOrdinate)ordinate).StandardDeviation;
        //    }
        //    _NormalControlVM.Mean = normalMean;

        //    //create the triangular option
        //    double triMostLikely = 1;
        //    double triMin = 0;
        //    double triMax = 2;
        //    if (ordType == IOrdinateEnum.Triangular)
        //    {
        //        triMostLikely = ((IDistributedOrdinate)ordinate).Mode;
        //        triMin = ((IDistributedOrdinate)ordinate).Range.Min;
        //        triMax = ((IDistributedOrdinate)ordinate).Range.Max;
        //    }
        //    _TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax);

        //    //create the uniform option
        //    double uniMin = 0;
        //    double uniMax = 1;
        //    if (ordType == IOrdinateEnum.Uniform)
        //    {
        //        uniMin = ((IDistributedOrdinate)ordinate).Range.Min;
        //        uniMax = ((IDistributedOrdinate)ordinate).Range.Max;
        //    }
        //    _UniformControlVM = new UniformControlVM(uniMin, uniMax);
        //}


        public void SelectedDistributionTypeChanged()
        {
            switch(_ValueUncertainty.Type)
            {
                case IDistributionEnum.Deterministic:
                    {
                        CurrentVM = null;//todo: ???
                        break;
                    }
                case IDistributionEnum.Normal:
                    {
                        
                        CurrentVM = _NormalControlVM;
                        break;
                    }
                case IDistributionEnum.Triangular:
                    {
                        CurrentVM = _TriangularControlVM;
                        break;
                    }
                case IDistributionEnum.Uniform:
                    {
                        CurrentVM = _UniformControlVM;
                        break;
                    }
                default:
                    {
                        CurrentVM = null;
                        break;
                    }
            }
        }

        public IDistribution CreateOrdinate()
        {
            if (CurrentVM == null)
            {
                //then it is a constant
                return new Deterministic(0); //todo: what value goes here?
            }
            else
            {
                return CurrentVM.CreateOrdinate();
            }
            //switch(_SelectedType)
            //{
            //    case IOrdinateEnum.Constant:
            //        {
            //            return new Constant(Mean);
            //        }
            //    case IOrdinateEnum.Normal:
            //        {
            //            return IDistributedOrdinateFactory.FactoryNormal(Mean, StDev);
            //        }
            //    case IOrdinateEnum.Triangular:
            //        {
            //            return IDistributedOrdinateFactory.FactoryTriangular(Mean, Min, Max);
            //        }
            //    case IOrdinateEnum.Uniform:
            //        {
            //            return IDistributedOrdinateFactory.FactoryUniform(Min, Max);
            //        }
            //}
            //throw new NotImplementedException("Did not recognize the selected type");
        }

    }
}
