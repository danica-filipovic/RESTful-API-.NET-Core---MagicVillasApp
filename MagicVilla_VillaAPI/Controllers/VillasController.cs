using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace MagicVilla_VillaAPI.Controllers
{

    [ApiController]
    public class VillasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;  

        public VillasController(ApplicationDbContext db)
        {
            _db = db;
        }


        [HttpGet()]
        [Route("api/Villas/GetAllVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            List<Villa> villas = _db.Villas.ToList();

            if (villas == null || !villas.Any())
            {
                return NotFound();
            };

            return Ok(villas);
        }


        [HttpGet()] 
        [Route("api/Villas/GetVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDTO> GetVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();

            }


            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);


            if (villa == null)
            {
                return NotFound();

            }


            return Ok(villa);
        }

        [HttpPost]
        [Route("api/Villas/CreateVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {

            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }


            Villa existingVilla = _db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower());

            if (existingVilla is not null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }



            Villa model = new()

            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                CreatedDate = DateTime.Now,
            };

            _db.Villas.Add(model);
            _db.SaveChanges();

            return Created($"api/Villas/GetVilla/{model.Id}", model);

        }


        [HttpDelete()]
        [Route("api/Villas/DeleteVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult DeleteVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }


            Villa villa = _db.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();


            return Ok();

        }




        [HttpPut()]
        [Route("api/Villas/UpdateVilla/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {

            if (villaDTO == null || id == 0)
            {
                return BadRequest();
            }

            Villa model = _db.Villas.FirstOrDefault(x => x.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            model.Name = villaDTO.Name;
            model.Amenity = villaDTO.Amenity;
            model.Details = villaDTO.Details;
            model.ImageUrl = villaDTO.ImageUrl;
            model.Rate = villaDTO.Rate;
            model.Sqft = villaDTO.Sqft;
            model.Occupancy = villaDTO.Occupancy;
            model.UpdatedDate = DateTime.Now;


            _db.Villas.Update(model);
            _db.SaveChanges();

            return Ok();
        }



        [HttpPatch()]
        [Route("api/Villas/UpdateVillaPartial/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        // [
        //  {
        //    "operationType": 5,
        //    "path": "/name",
        //    "op": "replace",
        //    "value": "Purple Villa"
        //  }
        //]


        public IActionResult UpdateVillaPartial(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {

            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            Villa villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();

            }

            VillaDTO villaDTO = new VillaDTO()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };

            patchDTO.ApplyTo(villaDTO, ModelState);

            if (!ModelState.IsValid)

            {
                return BadRequest(ModelState);
            }



            villa.Amenity = villaDTO.Amenity;
            villa.Details = villaDTO.Details;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Rate = villaDTO.Rate;
            villa.Sqft = villaDTO.Sqft;
            villa.UpdatedDate = DateTime.Now;




            _db.Villas.Update(villa);
            _db.SaveChanges();

            return Ok();
        }
    }
}