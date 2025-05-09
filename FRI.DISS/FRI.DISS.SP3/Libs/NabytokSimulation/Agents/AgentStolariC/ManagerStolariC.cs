using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariC
{
	//meta! id="13"
	public class ManagerStolariC : OSPABA.Manager
	{
		public ManagerStolariC(int id, OSPABA.Simulation mySim, Agent myAgent) :
			base(id, mySim, myAgent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

			if (PetriNet != null)
			{
				PetriNet.Clear();
			}
		}

		/*!
		 * akcia neposuva cas
		 * vrati null ak stolar nie je volny
		 * 
		 * zaroven nastavi daneho stolara na obsadeny
		 */
		//meta! sender="AgentStolarov", id="26", type="Request"
		public void ProcessRequestResponseDajStolara(MessageForm message)
		{
            var myMsg = (MyMessage)message;

            if (myMsg.Stolar is not null)
                throw new InvalidOperationException("Stolar must be null");

            if (MyAgent.FreeStolarai.Count > 0)
            {
               var stolar  = MyAgent.FreeStolarai.Dequeue();
               myMsg.Stolar = stolar;

               stolar.StartWork(MySim.CurrentTime);
            }

            Response(myMsg);
		}

		//meta! sender="AgentStolarov", id="41", type="Notice"
		public void ProcessNoticeStolarUvolneny(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            var stolar = myMsg.Stolar;

            if (stolar is null)
                throw new InvalidOperationException("Stolar cannot be null");

            stolar.StopWork(MySim.CurrentTime);
            MyAgent.FreeStolarai.Enqueue(stolar);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.NoticeStolarUvolneny:
				ProcessNoticeStolarUvolneny(message);
			break;

			case Mc.RequestResponseDajStolara:
				ProcessRequestResponseDajStolara(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentStolariC MyAgent
		{
			get
			{
				return (AgentStolariC)base.MyAgent;
			}
		}
	}
}
