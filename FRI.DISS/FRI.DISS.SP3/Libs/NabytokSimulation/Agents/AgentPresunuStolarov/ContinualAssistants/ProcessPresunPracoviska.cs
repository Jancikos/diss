using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.Libs.Generators;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov.ContinualAssistants
{
	//meta! id="64"
	public class ProcessPresunPracoviska : OSPABA.Process
	{
        private AbstractGenerator _genPresun;
        
		public ProcessPresunPracoviska(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            var seedGenerator = ((MySimulation)MySim).SeedGenerator;

            _genPresun = new TriangularGenerator(120, 500, 150, seedGenerator);
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentPresunuStolarov", id="65", type="Start"
		public void ProcessStart(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            myMsg.Code = Mc.Finish;

            var intime = _genPresun.GetSampleDouble();
            if (MySim.AnimatorExists)
            {
                var newPos = MyAnimator.GetStolarPosition(myMsg.Stolar!, myMsg.Nabytok!.Pracovisko!);
                myMsg.Stolar!.AnimShapeItem!.MoveTo(MySim.CurrentTime, intime, newPos.x, newPos.y);
            }
            Hold(intime, message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            AssistantFinished(message);
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
		public new AgentPresunuStolarov MyAgent
		{
			get
			{
				return (AgentPresunuStolarov)base.MyAgent;
			}
		}
	}
}
