using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using DAL.Concret;
using Entity.TableModel;
// using Entities.DataTransferObject.CategoryDTO; removed
// using static Entities.DataTransferObject.CategoryDTO.MessageListDTOCallListDTO; removed
using Entity.DataTransferObject.CallDTO;

namespace BLL.Concret
{
    public class CallManager : ICallService
    {
        private readonly ICallDAL _callDal;
        private readonly IMapper _mapper;
        public CallManager(ICallDAL callDal, IMapper mapper)
        {
            _callDal = callDal;
            _mapper = mapper;
        }
        public IResult Add(CallAddDTO callAddDTO)
        {
            var callMapper = _mapper.Map<Call>(callAddDTO);
            var validateValidator = new CallValidator();
            var validationResult = validateValidator.Validate(callMapper);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());


            // Duplicate Check Removed


            _callDal.Add(callMapper);
            return new SuccesResult("Added Succesfully");
        }

        public IResult Delete(int id) 
        {
            var callGet = _callDal.Get(x => x.Id == id);

            if (callGet is not null)
            {
                callGet.Deleted = id; 
                _callDal.Update(callGet);
                return new SuccesResult("Deleted Succesfully");
            }

            return new ErrorResult("Call Not found");


        }

        public IDataResult<CallListDTO> Get(int id)
        {
            var calls = _callDal.Get(x => x.Deleted == 0 && x.Id == id);
            return new SuccessDataResult<CallListDTO>(_mapper.Map<CallListDTO>(calls));
        }

        public IDataResult<List<CallListDTO>> GetAll()
        {
            var calls = _callDal.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<CallListDTO>>(_mapper.Map<List<CallListDTO>>(calls));
        }

        public IResult Update(CallUpdateDTO callUpdateDTO)
        {
            var callMapper = _mapper.Map<Call>(callUpdateDTO);

            var validateValidator = new CallValidator();
            var validationResult = validateValidator.Validate(callMapper);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());


            // Duplicate Check Removed

            _callDal.Update(callMapper);
            return new SuccesResult("Updated Succesfully");
        }



        // DuplicateCallName method removed
    }
}
