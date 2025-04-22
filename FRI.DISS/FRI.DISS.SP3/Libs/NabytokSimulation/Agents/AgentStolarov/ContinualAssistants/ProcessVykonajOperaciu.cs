using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.Helpers;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov.ContinualAssistants
{
	//meta! id="66"
	public class ProcessVykonajOperaciu : OSPABA.Process
	{
        protected Dictionary<NabytokType, Dictionary<NabytokOperation, AbstractGenerator>> _genOperations;

		public ProcessVykonajOperaciu(int id, OSPABA.Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            var seedGenerator = ((MySimulation)MySim).SeedGenerator;

            _genOperations = new()
            {
                { NabytokType.Stol, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new EmpiricalGenerator(GenerationMode.Continuous, [TimeHelper.M2S(10), TimeHelper.M2S(25), TimeHelper.M2S(50)], [0.6, 0.4], seedGenerator) },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(100), Max = TimeHelper.M2S(480)} },
                        { NabytokOperation.Lakovanie, new EmpiricalGenerator(GenerationMode.Continuous, [TimeHelper.M2S(50), TimeHelper.M2S(70), TimeHelper.M2S(150), TimeHelper.M2S(200)], [0.1, 0.6, 0.3], seedGenerator) },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(30), Max = TimeHelper.M2S(60)} }
                    }
                },
                { NabytokType.Stolicka, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(12), Max = TimeHelper.M2S(16)} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(90), Max = TimeHelper.M2S(400)} },
                        { NabytokOperation.Lakovanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(40), Max = TimeHelper.M2S(200)} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(14), Max = TimeHelper.M2S(24)} }
                    }
                },
                { NabytokType.Skrina, new Dictionary<NabytokOperation, AbstractGenerator>
                    {
                        { NabytokOperation.Rezanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(15), Max = TimeHelper.M2S(80)} },
                        { NabytokOperation.Morenie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(300), Max = TimeHelper.M2S(600)} },
                        { NabytokOperation.Lakovanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(250), Max = TimeHelper.M2S(560)} },
                        { NabytokOperation.Skladanie, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(35), Max = TimeHelper.M2S(75)} },
                        { NabytokOperation.MontazKovani, new UniformGenerator(GenerationMode.Continuous, seedGenerator) {Min = TimeHelper.M2S(15), Max = TimeHelper.M2S(25)} }
                    }
                }
            };
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentStolarov", id="67", type="Start"
		public void ProcessStart(MessageForm message)
		{
            var myMsg = (MyMessage)message;
            myMsg.Code = Mc.Finish;

            var intime = _genOperations[myMsg.Nabytok!.Type][myMsg.Nabytok!.MapStateToNextOperation()!].GetSampleDouble();
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
		public new AgentStolarov MyAgent
		{
			get
			{
				return (AgentStolarov)base.MyAgent;
			}
		}
	}
}
