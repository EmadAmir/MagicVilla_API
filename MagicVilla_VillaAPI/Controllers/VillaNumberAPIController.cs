using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            this._response = new APIResponse();
            _dbVilla = dbVilla;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber()
        {
            try 
            {
                IEnumerable<VillaNumber> villaNumbersList = await _dbVillaNumber.GetAllAsync(includeProperties:"Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex) 
            { 
                _response.ErrorMessages = new List<string>() { ex.ToString()};
                _response.IsSuccess = false;
                return _response; 
            }           
        }
        
        [HttpGet("{id:int}", Name ="GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {

            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);

                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try 
            {
                if (await _dbVillaNumber.GetAsync(x => x.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa number already exists");
                    return BadRequest(ModelState);
                }

                if (await _dbVilla.GetAsync(x => x.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "VillaID is not valid");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
                await _dbVillaNumber.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return _response;
            }
        }

        [HttpDelete("id:int", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try 
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);

                if (villaNumber == null)
                { 
                    _response.IsSuccess=false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _dbVillaNumber.RemoveAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };

            }
            return _response;
        }

        [HttpPut("id:int", Name ="UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try 
            {
                if (updateDTO != null && id != updateDTO.VillaNo)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _dbVilla.GetAsync(x => x.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "VillaID is not valid");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);

                await _dbVillaNumber.UpdateAsync(villaNumber);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex) 
            {
                _response.ErrorMessages = new List<string>() { ex.ToString()};
                _response.IsSuccess = false;    
               
            }
            return _response;
        }


    }
}
