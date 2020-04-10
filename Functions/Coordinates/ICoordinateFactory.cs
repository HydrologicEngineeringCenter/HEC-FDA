using Functions.Coordinates;
using Functions.Ordinates;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utilities.Serialization;

namespace Functions
{
    public static class ICoordinateFactory
    {
        public static ICoordinate Factory(double x, double y)
        {
            return new CoordinateConstants(new Constant(x),new Constant(y));
        }

        public static ICoordinate Factory(double x, Statistics.IDistribution y)
        {
            return new CoordinateVariableY(new Constant(x),new Distribution(y));
        }

        public static ICoordinate Factory(double x, IDistributedOrdinate y)
        {
            if (y is Distribution)
            {
                Constant c = new Constant(x);
                Distribution dist = (Distribution)y;
                CoordinateVariableY coord = new CoordinateVariableY(c, dist);
                return coord; //new CoordinateVariableY(new Constant(x), (Distribution)y);
            }
            else
            {
                //right now (2/19/2020), Distribution is the only thing that implements IDistributedOrdinate
                //so y has to be a Distribution. So we should never hit this.
                return null;
            }
        }

        private static XElement GetOrdinateChildElement(XElement element)
        {
            //This is kind of weird but there should only be one child element in an ordinates element.
            //I don't know what it will be called so i have to get it this way.
            IEnumerable<XElement> elements = element.Elements();
            foreach(XElement elem in elements)
            {
                return elem;
            }
            throw new ArgumentException("The ordinate XElement provided did not contain a child element.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xOrdinate">Should look like: <Ordinate Type="Constant"><Constant Value="1"/></Ordinate></param>
        /// <param name="yOrdinate"></param>
        /// <returns></returns>
        public static ICoordinate Factory(XElement xOrdinate, XElement yOrdinate)
        {
            //process the x ordinate
            IOrdinate xOrd = CreateOrdinate(xOrdinate);
            IOrdinate yOrd = CreateOrdinate(yOrdinate);

            if(xOrd.GetType().Equals(typeof(Constant)) && yOrd.GetType().Equals(typeof(Distribution)))
            {
                return new CoordinateVariableY((Constant)xOrd, (Distribution)yOrd);
            }
            else if (xOrd.GetType().Equals(typeof(Constant)) && yOrd.GetType().Equals(typeof(Constant)))
            {
                return new CoordinateConstants((Constant)xOrd, (Constant)yOrd);
            }
            else
            {
                throw new ArgumentException("Could not create a valid coordinate out of the xml elements passed in.");
            }

        }

        public static IOrdinate CreateOrdinate(XElement ordinateElement)
        {
            string ordinateType = ordinateElement.Attribute(SerializationConstants.TYPE).Value;
            if (ordinateType.Equals(SerializationConstants.CONSTANT))
            {
                XElement child = GetOrdinateChildElement(ordinateElement);
                double xVal = Convert.ToDouble(child.Attribute(SerializationConstants.VALUE).Value);
                return new Constant(xVal);
            }
            else
            {
                //its a distributed ordinate
                XElement child = GetOrdinateChildElement(ordinateElement);
                string childName = child.Name.LocalName;
                switch (childName)
                {
                    case SerializationConstants.NORMAL:
                        {
                            double mean = Convert.ToDouble(child.Attribute(SerializationConstants.MEAN).Value);
                            double stDev = Convert.ToDouble(child.Attribute(SerializationConstants.ST_DEV).Value);
                            int sample = Convert.ToInt32(child.Attribute(SerializationConstants.SAMPLE_SIZE).Value);
                            IDistribution distribution = IDistributionFactory.FactoryNormal(mean, stDev, sample);
                            return new Distribution(distribution);
                        }
                    case SerializationConstants.TRIANGULAR:
                        {
                            double min = Convert.ToDouble(child.Attribute(SerializationConstants.MIN).Value);
                            double mostLikely = Convert.ToDouble(child.Attribute(SerializationConstants.MODE).Value);
                            double max = Convert.ToDouble(child.Attribute(SerializationConstants.MAX).Value);
                            int sample = Convert.ToInt32(child.Attribute(SerializationConstants.SAMPLE_SIZE).Value);
                            IDistribution distribution = IDistributionFactory.FactoryTriangular(min, mostLikely, max, sample);
                            return new Distribution(distribution);
                        }
                    case SerializationConstants.UNIFORM:
                        {
                            double min = Convert.ToDouble(child.Attribute(SerializationConstants.MIN).Value);
                            double max = Convert.ToDouble(child.Attribute(SerializationConstants.MAX).Value);
                            int sample = Convert.ToInt32(child.Attribute(SerializationConstants.SAMPLE_SIZE).Value);
                            IDistribution distribution = IDistributionFactory.FactoryUniform(min, max, sample);
                            return new Distribution(distribution);
                        }
                }
            }
            throw new ArgumentException("Could not create an ordinate out of the XElement passed in.");
        }

    }
}
