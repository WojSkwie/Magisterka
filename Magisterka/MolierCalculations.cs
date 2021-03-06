﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public static class MolierCalculations
    {
        public static double FindAirDensity(Air air)
        {
            //y = 1.289429 - 0.004834286*x + 0.00001571429*x^2
            /*double C = 1.289429;
            double B = 0.004834286;
            double A = 0.00001571429;
            return (A * air.Temperature * air.Temperature + B * air.Temperature + C);*/
            const double Ra = 286.9;
            const double Rw = 461.5;
            double density = 101325 / (Ra * (air.Temperature + 273)) * (1 + air.SpecificHumidity) / (1 + air.SpecificHumidity * Ra / Rw);
            return density;
        }

        public static double CalculateSaturationVaporPressure(double temperature)
        {
            //double Es = 611.2 * Math.Exp(17.67 * (temperature) / (temperature + 273.16 - 29.65));
            double Es = 610.5 * Math.Exp(17.269 * (temperature) / (temperature + 237.3));
            return Es;
        }

        public static double HumiditySpecificToRelative(Air air)
        {
            double Es = CalculateSaturationVaporPressure(air.Temperature);  //611.2 * Math.Exp(17.67 * (air.Temperature) / (air.Temperature + 273.16 - 29.65));
            double E = air.SpecificHumidity * 101325.0 / (0.378 * air.SpecificHumidity + 0.622);
            double Relative = E / Es;
            return Relative * 100.0;
        }

        public static double HumidityRelativeToSpecific(Air air)
        {
            double Es = CalculateSaturationVaporPressure(air.Temperature);//6.122 * Math.Exp
            double E = air.RelativeHumidity / 100.0 * Es;
            double p_mb = 101325.0;// 100.0;
            //double Specific = 0.6222 * E / (p_mb - E);
            double Specific = (0.6222 * E) / (p_mb - (0.378 * E));
            return Specific;

        }

        /// <returns>Dew point as temperature</returns>
        public static double CalculateDewPoint(Air air)
        {
            double A = (Math.Log(air.RelativeHumidity / 100) / 17.27 + air.Temperature / (237.3 + air.Temperature));
            double Nominator = 237.3 * A;
            double Denominator = 1 - A;
            double DewPoint = Nominator / Denominator;
            return DewPoint;
        }

        public static double CalculateEnthalpy(Air air)
        {
            double dryAirElthalpy = 1.007 * air.Temperature - 0.026;
            double vaporEnthalpy = air.SpecificHumidity * (2501 + 1.84 * air.Temperature);
            return vaporEnthalpy + dryAirElthalpy;
        }
    }
}
