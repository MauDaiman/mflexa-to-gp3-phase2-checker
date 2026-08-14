using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using System.Net.NetworkInformation;
using NationalInstruments.DAQmx;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
	public class InstrumentConfig
	{
		public static void InstrumentSetup(ISemiconductorModuleContext tsmContext)
		{
				Globals.tsmContext = tsmContext;

				NIDCPower[]
				dcPowerSessions;

				string[] dcPowerChannelStrings;
				Globals.tsmContext.GetNIDCPowerSessions("AllDC", out dcPowerSessions, out dcPowerChannelStrings);
				for (int i = 0; i < dcPowerSessions.Length; i++)
				{
					var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
					{
						output.Control.Abort();
						output.Measurement.ApertureTime = 0.000001;
						output.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);
						output.Control.Initiate();
					}

				}

				var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
				var digitalssc = sessions.SSC;

				sessions.Abort();
				sessions.PPMUConfigureApertureTime(0.000004);

		}
    }
}
