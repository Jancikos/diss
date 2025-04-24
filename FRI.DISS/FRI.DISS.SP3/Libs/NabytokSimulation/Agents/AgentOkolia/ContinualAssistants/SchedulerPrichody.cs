using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.Helpers;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;

namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia.ContinualAssistants
{
	//meta! id="68"
	public class SchedulerPrichody : OSPABA.Scheduler
	{
        private AbstractGenerator _genPrichody;

        private AbstractGenerator _genPocetNabytkov;
        private AbstractGenerator _genTypNabytku;
        private AbstractGenerator _genLakovanie;

		public SchedulerPrichody(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            var seedGenerator = ((MySimulation)MySim).SeedGenerator;

            // 2 objednavky za hodinu
            _genPrichody = new ExponentialGenerator(1.0 /
        (double)TimeHelper.M2S(30), seedGenerator);

            // rovnomerne rozdelenie 1-5
            _genPocetNabytkov = new UniformGenerator(GenerationMode.Discrete, seedGenerator)
            {
                Min = 1,
                Max = 6
            };

            // rovnomerne rozdelenie 0-1
            _genTypNabytku = new UniformGenerator(GenerationMode.Continuous, seedGenerator);
            _genLakovanie = new UniformGenerator(GenerationMode.Continuous, seedGenerator);
		}

        protected NabytokType _generateNabytokType()
            {
                var p = _genTypNabytku.GetSampleDouble();

                if (p <= 0.50)
                    return NabytokType.Stol;

                if (p <= 0.65)
                    return NabytokType.Stolicka;

                return NabytokType.Skrina;
            }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentOkolia", id="69", type="Start"
		public void ProcessStart(MessageForm message)
		{
            var inTime = _genPrichody.GetSampleDouble();
            message.Code = Mc.Finish;
            Hold(inTime, message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            var ntcMessage = new MyMessage(MySim)
            {
                Objednavka = _createNewOrder()
            };
            AssistantFinished(ntcMessage);
		}

        private Objednavka _createNewOrder()
        {
            var objednavka = new Objednavka();

            var pocetNabytkov = _genPocetNabytkov.GetSampleInt();
            for (int i = 0; i < pocetNabytkov; i++)
            {
                objednavka.Nabytky.Add(new Nabytok(objednavka, _generateNabytokType()));
            }

            return objednavka;
        }

        //meta! userInfo="Generated code: do not modify", tag="begin"
        override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Start:
				ProcessStart(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentOkolia MyAgent
		{
			get
			{
				return (AgentOkolia)base.MyAgent;
			}
		}
	}
}
