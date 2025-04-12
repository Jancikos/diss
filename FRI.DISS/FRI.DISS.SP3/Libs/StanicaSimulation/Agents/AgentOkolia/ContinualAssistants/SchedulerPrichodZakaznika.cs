using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia;
using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using FRI.DISS.Libs.Generators;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia.ContinualAssistants
{
	//meta! id="33"
	public class SchedulerPrichodZakaznika : OSPABA.Scheduler
	{
        private AbstractGenerator _generator = new ExponentialGenerator(1.0 / 10.0, SeedGenerator.Global);

		public SchedulerPrichodZakaznika(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentOkolia", id="34", type="Start"
		public void ProcessStart(MessageForm message)
		{
            message.Code = Mc.NoticePrichodZakaznika;
            Hold(_generator.GetSampleDouble(), message);
		}

        public void ProcessNoticePrichodZakaznika(MessageForm message)
        {
            message.Addressee = MyAgent;
            Notice(message);
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
                case Mc.NoticePrichodZakaznika:
                    ProcessNoticePrichodZakaznika(message);
                    break;
			}
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