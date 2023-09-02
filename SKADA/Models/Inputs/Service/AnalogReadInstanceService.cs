using SKADA.Models.Inputs.Model;
using SKADA.Models.Inputs.Repository;

namespace SKADA.Models.Inputs.Service
{
    public class AnalogReadInstanceService : IAnalogReadInstanceService
    {
        private readonly IAnalogReadInstanceRepository _analogReadInstanceRepository;
        private readonly IAnalogInputRepository _analogInputRepository;
        public AnalogReadInstanceService(IAnalogReadInstanceRepository analogReadInstanceRepository, IAnalogInputRepository analogInputRepository)
        {
            _analogReadInstanceRepository = analogReadInstanceRepository;
            _analogInputRepository = analogInputRepository;
        }
        public async Task Create(AnalogReadInstance input)
        {
            await _analogReadInstanceRepository.Create(input);
        }

        public Task<AnalogReadInstance> Get(Guid id)
        {
            return _analogReadInstanceRepository.Get(id);
        }

        public Task<AnalogReadInstance> GetByTagId(Guid id)
        {
            return _analogReadInstanceRepository.GetByTagId(id);
        }

        public async Task<IEnumerable<AnalogReadInstance>> GetAllAnalogReads()
        {
            IEnumerable<AnalogInput> analogInputs = await _analogInputRepository.GetAll();
            
            List<AnalogReadInstance> lastAnalogReads = new List<AnalogReadInstance>();
            foreach(AnalogInput analogInput in analogInputs)
            {
                IEnumerable<AnalogReadInstance> digitalReadInstances = await _analogReadInstanceRepository.GetAllSorted(analogInput.Id);
                lastAnalogReads.Add(digitalReadInstances.First());
            }

            lastAnalogReads = lastAnalogReads.OrderBy(x => x.Timestamp).ToList();


            return lastAnalogReads;
        }
    }
}
