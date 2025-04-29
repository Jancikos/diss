using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu
{
	/*!
	 * Spracovava zoznam objednavok a stara sa o postupne vykonavanie prace na objednavkach
	 */
	//meta! id="1"
	public class ManagerModelu : OSPABA.Manager
	{
		public ManagerModelu(int id, OSPABA.Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentOkolia", id="8", type="Notice"
		public void ProcessNoticePrichodObjednavka(MessageForm message)
		{
            if (message.Code != Mc.NoticePrichodObjednavka)
                throw new InvalidOperationException("Invalid message code");

            var myMsg = (MyMessage)message;
            // pridaj objednavku do zoznamu
            var objednavka = myMsg.Objednavka;
            if (objednavka is null)
                throw new InvalidOperationException("Objednavka cannot be null");

            MyAgent.ObjednavkyTotal.Add(objednavka);

            // prirad nabytkom pracovisko
            foreach (var nabytok in objednavka.Nabytky)
            {
                var nabytokMsg = (MyMessage)myMsg.CreateCopy();
                nabytokMsg.Nabytok = nabytok;
                nabytokMsg.Code = Mc.RequestResponsePriradPracovisko;
                nabytokMsg.Addressee = MySim.FindAgent(SimId.AgentPracovisk);
                Request(nabytokMsg);
            }
        }

		/*!
		 * posle oparaciu, ktoru chce vykonat
		 * 
		 */
		//meta! sender="AgentStolarov", id="22", type="Response"
		public void ProcessRequestResponseVykonajOperaciu(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            if (myMsg.Nabytok is null)
                throw new InvalidOperationException("Nabytok cannot be null");

            if (MySim.AnimatorExists)
                myMsg.Nabytok.Pracovisko!.Rerender(MySim.Animator);

            if (myMsg.Nabytok.AllWorkDone)
            {
                // nabytok je hotovy callback
                _handleNabytokFinished(myMsg.Nabytok!);
                return;
            }

            // naplanuj vykonanie dalejsej operacie
            myMsg.Addressee = MySim.FindAgent(SimId.AgentStolarov);
            myMsg.Code = Mc.RequestResponseVykonajOperaciu;
            Request(myMsg);
		}

        private void _handleNabytokFinished(Nabytok nabytok)
        {
            if (!nabytok.AllWorkDone) 
                throw new InvalidOperationException("Nabytok is not finished");

            var objednavka = nabytok.Objednavka!;

            // aktualiuj statistiky
            objednavka.NabytokDoneCount++;

            // uvolni pracovisko
            var myMsg = new MyMessage(MySim)
            {
                Pracovisko = nabytok.Pracovisko,
                Code = Mc.NoticePracoviskoUvolnene,
                Addressee = MySim.FindAgent(SimId.AgentPracovisk)
            };
            Notice(myMsg);

            // skontroluj ci je uz nie cela objednavka hotova
            if (objednavka.NabytokDoneCount == objednavka.NabytokCount)
            {
                _handleObjednavkaFinished(objednavka);
            }
        }

        private void _handleObjednavkaFinished(Objednavka objednavka)
        {
            MyAgent.ObjednavkyDoneCount++;
            objednavka.EndTime = MySim.CurrentTime;
            MyAgent.ObjednavkaTotalTime.AddSample(objednavka.TimeInSystem);
        }

        /*!
		 * request na priradene pracovbisku sa moze spravit len ak ten agent pracovisk  
		 * 
		 * 
		 * meisto sa moze 
		 */
        //meta! sender="AgentPracovisk", id="32", type="Response"

        public void ProcessRequestResponsePriradPracovisko(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            if (message.Code != Mc.RequestResponsePriradPracovisko)
                throw new InvalidOperationException("Invalid message code");

            // posli operaciu na vykonanie
            myMsg.Code = Mc.RequestResponseVykonajOperaciu;
            ProcessRequestResponseVykonajOperaciu(myMsg);
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
			case Mc.RequestResponsePriradPracovisko:
				ProcessRequestResponsePriradPracovisko(message);
			break;

			case Mc.RequestResponseVykonajOperaciu:
				ProcessRequestResponseVykonajOperaciu(message);
			break;

			case Mc.NoticePrichodObjednavka:
				ProcessNoticePrichodObjednavka(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentModelu MyAgent
		{
			get
			{
				return (AgentModelu)base.MyAgent;
			}
		}
	}
}
