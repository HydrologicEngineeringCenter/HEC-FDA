using Functions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// Note that there are only 5 of these in the occtype editor. There is NOT 5 per occtype.
    /// That means that these need to get updated with the new values everytime the occtype changes.
    /// </summary>
    public class ValueUncertaintyVM:BaseViewModel
    {
        private IOrdinate _ValueUncertainty;
       // private IOrdinateEnum _SelectedType;
        private IValueUncertainty _CurrentVM;
        private NormalControlVM _NormalControlVM;
        private TriangularControlVM _TriangularControlVM;
        private UniformControlVM _UniformControlVM;
        private ValueUncertaintyType _valueUncertaintyType;

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


        public ObservableCollection<IOrdinateEnum> UncertaintyTypes
        {
            get;
            set;
        }

        //public IOrdinateEnum SelectedType
        //{
        //    get
        //    {
        //        return _SelectedType;
        //    }
        //    set
        //    {
        //        _SelectedType = value;
        //        NotifyPropertyChanged();
        //        SelectedDistributionTypeChanged();
        //    }
        //}

        public IOrdinate ValueUncertainty
        {
            get
            {
                return _ValueUncertainty;
            }
            set
            {
                _ValueUncertainty = value;
                //SelectedType = _ValueUncertainty.Type;
                UpdateDistributionValues(value);
                SelectedDistributionTypeChanged();
                NotifyPropertyChanged();
            }
        }

        //public double Min
        //{
        //    get;
        //    set;
        //}

        //public double Max
        //{
        //    get;
        //    set;
        //}

        //public double StDev
        //{
        //    get;
        //    set;
        //}

        //public double Mean
        //{
        //    get;
        //    set;
        //}
            

        public ValueUncertaintyVM(IOrdinate valueUncertaintyOrdinate, ValueUncertaintyType valueUncertaintyType)
        {
            ValueUncertaintyType = ValueUncertaintyType;
            //create the options for the combobox
            UncertaintyTypes = new ObservableCollection<IOrdinateEnum>()
            {
                IOrdinateEnum.Constant,
                IOrdinateEnum.Normal,
                IOrdinateEnum.Triangular,
                IOrdinateEnum.Uniform
            };

           

            //create the vm's for the individual distribution types

            //set what values you can, then set some defaults for the other dist types?
            CreateDistributionControls(valueUncertaintyOrdinate);

            ValueUncertainty = valueUncertaintyOrdinate;

            //set the current vm to be of the selected tyep
            SelectedDistributionTypeChanged();

        }

        /// <summary>
        /// Creates all the distribution control view models. The occtype should
        /// have a value for each of the assets. I set those and then create default
        /// data for the other types so that when the user makes a different selection
        /// in the combobox, a view model with default values is displayed
        /// </summary>
        /// <param name="ordinate"></param>
        private void CreateDistributionControls(IOrdinate ordinate)
        {
            IOrdinateEnum ordType = ordinate.Type;
            //create constant option
            if(ordType == IOrdinateEnum.Constant)
            {
                //todo: i don't really know how to handle constant right now
            }

            //create normal option
            double normalMean = 0;
            double normalStDev = 0;
            if(ordType == IOrdinateEnum.Normal)
            {
                 normalMean = ((IDistributedOrdinate)ordinate).Mean;
                normalStDev = ((IDistributedOrdinate)ordinate).StandardDeviation;
            }
            _NormalControlVM = new NormalControlVM(normalMean, normalStDev);

            //create the triangular option
            double triMostLikely = 1;
            double triMin = 0;
            double triMax = 2;
            if(ordType == IOrdinateEnum.Triangular)
            {
                triMostLikely = ((IDistributedOrdinate)ordinate).Mode;
                triMin = ((IDistributedOrdinate)ordinate).Range.Min;
                triMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax);

            //create the uniform option
            double uniMin = 0;
            double uniMax = 1;
            if(ordType == IOrdinateEnum.Uniform)
            {
                uniMin = ((IDistributedOrdinate)ordinate).Range.Min;
                uniMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _UniformControlVM = new UniformControlVM(uniMin, uniMax);

        }

        private void UpdateDistributionValues(IOrdinate ordinate)
        {
            IOrdinateEnum ordType = ordinate.Type;
            //create constant option
            if (ordType == IOrdinateEnum.Constant)
            {
                //todo: i don't really know how to handle constant right now
            }

            //create normal option
            double normalMean = 0;
            double normalStDev = 0;
            if (ordType == IOrdinateEnum.Normal)
            {
                normalMean = ((IDistributedOrdinate)ordinate).Mean;
                normalStDev = ((IDistributedOrdinate)ordinate).StandardDeviation;
            }
            _NormalControlVM.Mean = normalMean;

            //create the triangular option
            double triMostLikely = 1;
            double triMin = 0;
            double triMax = 2;
            if (ordType == IOrdinateEnum.Triangular)
            {
                triMostLikely = ((IDistributedOrdinate)ordinate).Mode;
                triMin = ((IDistributedOrdinate)ordinate).Range.Min;
                triMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax);

            //create the uniform option
            double uniMin = 0;
            double uniMax = 1;
            if (ordType == IOrdinateEnum.Uniform)
            {
                uniMin = ((IDistributedOrdinate)ordinate).Range.Min;
                uniMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _UniformControlVM = new UniformControlVM(uniMin, uniMax);
        }


        private void SelectedDistributionTypeChanged()
        {
            switch(_ValueUncertainty.Type)
            {
                case IOrdinateEnum.Constant:
                    {
                        CurrentVM = null;//todo: ???
                        break;
                    }
                case IOrdinateEnum.Normal:
                    {
                        
                        CurrentVM = _NormalControlVM;
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        CurrentVM = _TriangularControlVM;
                        break;
                    }
                case IOrdinateEnum.Uniform:
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

        public IOrdinate CreateOrdinate()
        {
            if (CurrentVM == null)
            {
                //then it is a constant
                return new Constant(0); //todo: what value goes here?
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
