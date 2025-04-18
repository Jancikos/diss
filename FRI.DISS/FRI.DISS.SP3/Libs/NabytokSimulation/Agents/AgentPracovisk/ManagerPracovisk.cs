using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk
{
	/*!
	 * pokusi sa priradit pracovisko pre dany nabytok
	 * 
	 * ak nema aktualne volne pracovisko, tak ziadost zaradi do radu
	 * 
	 * 
	 */
	//meta! id="30"
	public class ManagerPracovisk : OSPABA.Manager
	{
		public ManagerPracovisk(int id, OSPABA.Simulation mySim, Agent myAgent) :
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
		 * request na priradene pracovbisku sa moze spravit len ak ten agent pracovisk  
		 * 
		 * 
		 * meisto sa moze 
		 */
		//meta! sender="AgentModelu", id="32", type="Request"
		public void ProcessRequestResponsePriradPracovisko(MessageForm message)
		{
            if (message.Code != Mc.RequestResponsePriradPracovisko)
            {
                throw new InvalidOperationException("Invalid message code");
            }

            var myMsg = (MyMessage)message;

            // pokus sa priradit pracovisko
            if (!MyAgent.HasFreePracovisko)
            {
                // ak nie je volne pracovisko, tak ziadost zarad do radu
                MyAgent.WaitingForPracovisko.Enqueue(myMsg);
                return;
            }

            // ak je volne pracovisko, tak pridel pracovisko
            var pracovisko = MyAgent.FreePracoviska.Dequeue();
            pracovisko.CurrentNabytok = myMsg.Nabytok;
            myMsg.Pracovisko = pracovisko;
            Response(myMsg);
		}

		//meta! sender="AgentModelu", id="34", type="Notice"
		public void ProcessNoticePracoviskoUvolnene(MessageForm message)
		{
            if (message.Code != Mc.NoticePracoviskoUvolnene)
                throw new InvalidOperationException("Invalid message code");

            var myMsg = (MyMessage)message;

            // uvolni pracovisko
            var pracovisko = myMsg.Pracovisko;
            if (pracovisko is null)
                throw new InvalidOperationException("Pracovisko cannot be null");

            // ak je niekto v rade, tak mu pridel pracovisko
            if (MyAgent.HasWaitingForPracovisko)
            {
                var waitingMsg = MyAgent.WaitingForPracovisko.Dequeue();
                waitingMsg.Pracovisko = pracovisko;
                Response(waitingMsg);
                return;
            }

            // ak nikto nie je v rade, tak pracovisko je volne
            MyAgent.FreePracoviska.Enqueue(pracovisko);
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
			case Mc.NoticePracoviskoUvolnene:
				ProcessNoticePracoviskoUvolnene(message);
			break;

			case Mc.RequestResponsePriradPracovisko:
				ProcessRequestResponsePriradPracovisko(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentPracovisk MyAgent
		{
			get
			{
				return (AgentPracovisk)base.MyAgent;
			}
		}
	}
}
