using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika;
using FRI.DISS.Libs.Generators;


namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.ContinualAssistants
{
	//meta! id="38"
	public class ProcessObsluzZakaznika : OSPABA.Process
	{
        protected AbstractGenerator _generator = new ExponentialGenerator(1.0 / (4.0 * 60.0), SeedGenerator.Global);

		public ProcessObsluzZakaznika(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentObsluhaZakaznika", id="39", type="Start"
		public void ProcessStart(MessageForm message)
		{
            message.Code = Mc.RequestResponseObluzZakaznika;
            Hold(_generator.GetSampleDouble(), message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
                case Mc.RequestResponseObluzZakaznika:
                    ProcessRequestResponseObluzZakaznika(message);
                    break;
			}
		}

        private void ProcessRequestResponseObluzZakaznika(MessageForm message)
        {
            if (message.Code != Mc.RequestResponseObluzZakaznika)
            {
                throw new InvalidOperationException("Invalid message code");
            }

            // notifikuj parku o prichode zakaznika
            message.Addressee = MyAgent;
            AssistantFinished(message.CreateCopy());
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
		public new AgentObsluhaZakaznika MyAgent
		{
			get
			{
				return (AgentObsluhaZakaznika)base.MyAgent;
			}
		}
	}
}