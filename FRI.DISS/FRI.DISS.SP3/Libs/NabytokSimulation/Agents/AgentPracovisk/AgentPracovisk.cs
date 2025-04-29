using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using OSPAnimator;
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
	public class AgentPracovisk : OSPABA.Agent, IAnimatoredAgent
	{
        public List<Pracovisko> Pracoviska { get; set; } = new();

        public Queue<Pracovisko> FreePracoviska { get; set; } = new();
        public bool HasFreePracovisko => FreePracoviska.Count > 0;


        public Queue<MyMessage> WaitingForPracovisko { get; set; } = new();
        public bool HasWaitingForPracovisko => WaitingForPracovisko.Count > 0;

		public AgentPracovisk(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

            Pracoviska.Clear();
            FreePracoviska.Clear();
            WaitingForPracovisko.Clear();

            for (int i = 0; i < ((MySimulation)MySim).PracoviskaCount; i++)
            {
                var pracovisko = new Pracovisko();
                Pracoviska.Add(pracovisko);
                FreePracoviska.Enqueue(pracovisko);
            }
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerPracovisk(SimId.ManagerPracovisk, MySim, this);
			AddOwnMessage(Mc.RequestResponsePriradPracovisko);
			AddOwnMessage(Mc.NoticePracoviskoUvolnene);
		}

        public void InitializeSimulationAnimator(IAnimator oldAnimator, IAnimator newAnimator)
        {
            // initialize new animator
            Pracovisko.Sklad.Initialize(newAnimator);
            foreach (var pracovisko in Pracoviska)
            {
                pracovisko.Initialize(newAnimator);
            }
        }

        public void DestroySimulationAnimator(IAnimator oldAnimator)
        {
            // destroy old animator
            Pracovisko.Sklad.Destroy(oldAnimator);
            foreach (var pracovisko in Pracoviska)
            {
                pracovisko.Destroy(oldAnimator);
            }
        }
        //meta! tag="end"
    }
}
