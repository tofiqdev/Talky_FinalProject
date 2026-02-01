using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.TableModel;
using Entity.DataTransferObject.StoryViewDTO;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concret
{
    public class StoryViewManager : IStoryViewService
    {
        private readonly IStoryViewDAL _storyViewDAL;
        private readonly IMapper _mapper;

        public StoryViewManager(IStoryViewDAL storyViewDAL, IMapper mapper)
        {
            _storyViewDAL = storyViewDAL;
            _mapper = mapper;
        }

        public IResult Add(StoryViewAddDTO storyViewAddDTO)
        {
            var storyView = _mapper.Map<StoryView>(storyViewAddDTO);
            storyView.ViewedAt = DateTime.UtcNow;
            
            StoryViewValidator validator = new StoryViewValidator();
            ValidationResult result = validator.Validate(storyView);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            // Check if user already viewed this story
            var existingView = _storyViewDAL.Get(x => x.StoryId == storyView.StoryId && x.UserId == storyView.UserId && x.Deleted == 0);
            
            if (existingView != null)
            {
                // Update the viewed time
                existingView.ViewedAt = DateTime.UtcNow;
                _storyViewDAL.Update(existingView);
                return new SuccesResult("Hikaye görüntüleme zamanı güncellendi.");
            }

            _storyViewDAL.Add(storyView);
            return new SuccesResult("Hikaye görüntüleme kaydedildi.");
        }

        public IResult Delete(int id)
        {
            var storyView = _storyViewDAL.Get(x => x.Id == id);
            if (storyView == null)
            {
                return new ErrorResult("Görüntüleme kaydı bulunamadı.");
            }

            storyView.Deleted = id;
            _storyViewDAL.Update(storyView);
            return new SuccesResult("Görüntüleme kaydı silindi.");
        }

        public IDataResult<List<StoryView>> GetAll()
        {
            var data = _storyViewDAL.GetAll(x => x.Deleted == 0).ToList();
            return new SuccessDataResult<List<StoryView>>(data, "Görüntüleme kayıtları listelendi.");
        }

        public IDataResult<StoryView> GetById(int id)
        {
            var data = _storyViewDAL.Get(x => x.Id == id && x.Deleted == 0);
            if (data == null)
            {
                return new ErrorDataResult<StoryView>("Görüntüleme kaydı bulunamadı.");
            }
            return new SuccessDataResult<StoryView>(data, "Görüntüleme kaydı getirildi.");
        }

        public IResult Update(StoryView storyView)
        {
            StoryViewValidator validator = new StoryViewValidator();
            ValidationResult result = validator.Validate(storyView);

            if (!result.IsValid)
            {
                return new ErrorResult(result.ConvertToString());
            }

            _storyViewDAL.Update(storyView);
            return new SuccesResult("Görüntüleme kaydı güncellendi.");
        }
    }
}
