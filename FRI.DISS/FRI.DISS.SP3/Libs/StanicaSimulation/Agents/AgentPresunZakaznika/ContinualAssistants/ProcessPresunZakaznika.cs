using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika;
using FRI.DISS.Libs.Generators;


namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika.ContinualAssistants
{
	//meta! id="42"
	public class ProcessPresunZakaznika : OSPABA.Process
	{
        protected AbstractGenerator _generator = new ExponentialGenerator(1.0 / 20.0, SeedGenerator.Global);

		public ProcessPresunZakaznika(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentPresunZakaznika", id="43", type="Start"
		public void ProcessStart(MessageForm message)
		{
            message.Code = Mc.RequestResponsePresunZakaznika;
            var inTime = _generator.GetSampleDouble();
            Hold(inTime, message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
                case Mc.RequestResponsePresunZakaznika:
                    ProcessRequestResponsePresunZakaznika(message);
                    break;
			}
		}

        private void ProcessRequestResponsePresunZakaznika(MessageForm message)
        {
            var msg = message.CreateCopy();
            msg.Addressee = MyAgent.Parent;
            
            Notice(msg);
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
		public new AgentPresunZakaznika MyAgent
		{
			get
			{
				return (AgentPresunZakaznika)base.MyAgent;
			}
		}
	}
}