using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.TableModel;
using Entity.DataTransferObject.StoryDTO;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concret
{
    public class StoryManager : IStoryService
    {
        private readonly IStoryDAL _storyDAL;
        private readonly IMapper _mapper;

        public StoryManager(IStoryDAL storyDAL, IMapper mapper)
        {
            _storyDAL = storyDAL;
            _mapper = mapper;
        }

        public IDataResult<StoryListDTO> Add(StoryAddDTO storyAddDTO)
        {
            var story = _mapper.Map<Story>(storyAddDTO);
            
            StoryValidator validator = new StoryValidator();
            ValidationResult result = validator.Validate(story);

            if (!result.IsValid)
            {
                return new ErrorDataResult<StoryListDTO>(result.ConvertToString());
            }

            _storyDAL.Add(story);
            var dto = _mapper.Map<StoryListDTO>(story);
            return new SuccessDataResult<StoryListDTO>(dto, "Hikaye başarıyla paylaşıldı.");
        }

        public IResult Delete(int id)
        {
            var story = _storyDAL.Get(x => x.Id == id);
            if (story == null)
            {
                return new ErrorResult("Hikaye bulunamadı.");
            }

            story.Deleted = id;
            _storyDAL.Update(story);
            return new SuccesResult("Hikaye başarıyla silindi.");
        }

        public IDataResult<List<Story>> GetAll()
        {
            var data = _storyDAL.GetAll(x => x.Deleted == 0 && x.ExpiresAt > DateTime.UtcNow).ToList();
            return new SuccessDataResult<List<Story>>(data, "Aktif hikayeler listelendi.");
        }

        public IDataResult<Story> Get(int id)
        {
            var data = _storyDAL.Get(x => x.Id == id && x.Deleted == 0);
            if (data == null)
            {
                return new ErrorDataResult<Story>("Hikaye bulunamadı.");
            }
            return new SuccessDataResult<Story>(data, "Hikaye getirildi.");
        }

        public IDataResult<Story> GetById(int id)
        {
            return Get(id);
        }

        public IResult Update(Story story)
        {
            StoryValidator validator = new StoryValidator();
            ValidationResult result = validator.Validate(story);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            _storyDAL.Update(story);
            return new SuccesResult("Hikaye güncellendi.");
        }
    }
}
