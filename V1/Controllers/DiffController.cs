using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DiffingApi.Entities;
using DiffingApi.Services;
using DiffingApi.V1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiffingApi.V1.Controllers
{
    [ApiVersion("0.9", Deprecated = true)]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IImagesRepository _imageRepository;
        private readonly ILogger<DiffController> _logger;
        public DiffController(ILogger<DiffController> logger, IImagesRepository imageRepository)
        {
            _logger = logger;
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        #region Testing
        //[ApiVersion("1.0")]
        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    return new OkObjectResult("Diff from v1 controller");
        //}

        //[MapToApiVersion("0.9")]
        //[HttpGet("{id}")]
        //public IActionResult GetById09(int id)
        //{
        //    return new OkObjectResult("Diff from v0.9 action");
        //} 
        #endregion

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Difference), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var image = await _imageRepository.GetAsync(id);
            if (image != null)
            {
                //if we have both left and right content we do compare
                if (!string.IsNullOrWhiteSpace(image.ContentLeft) && !string.IsNullOrWhiteSpace(image.ContentRight))
                {
                    bool differenceExist = false;
                    var result = new Result();

                    //do diffing and return result
                    byte[] byteArrayLeft = Convert.FromBase64String(image.ContentLeft);
                    byte[] byteArrayRight = Convert.FromBase64String(image.ContentRight);

                    //left and right content are the same size so we iterate over data and do diffing
                    if (byteArrayRight.Length == byteArrayLeft.Length)
                    {
                        for (int i = 0; i < byteArrayLeft.Length - 1; i++)
                        {
                            //checking difference
                            var left = byteArrayLeft.ElementAt(i);
                            var right = byteArrayRight.ElementAt(i);


                            //difference exist
                            if (left != right)
                            {
                                //in case there were no difference before so we need to initialize collection
                                if (result.Diffs == null)
                                {
                                    result.Diffs = new List<Difference>();
                                }

                                //in case it is start of difference we set flag
                                if (!differenceExist)
                                {
                                    //we set flag
                                    differenceExist = true;
                                    result.Diffs.Add(new Difference() { Offset = i, Length = 1 });
                                }
                                else
                                {
                                    //if difference is repeating we are increasing length of difference
                                    result.Diffs.Last().Length += 1;
                                }
                            }
                            else
                            {
                                if (differenceExist)
                                {
                                    //if difference stops we clear flag
                                    differenceExist = false;
                                }
                            }
                        }

                        //if there were no differences diffs will be null
                        result.DiffResultType = result.Diffs == null ? DiffResultType.Equals.ToString() : DiffResultType.ContentDoNotMatch.ToString();

                        //serializing response to lower case and removing empty data
                        var message = JsonSerializer.Serialize(result, new JsonSerializerOptions()
                        {
                            IgnoreNullValues = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        return Ok(message);
                    }
                    else
                    {
                        //left and right content are not same size so we guess they are not the similar
                        result.DiffResultType = DiffResultType.SizeDoNotMatch.ToString();
                        var message = JsonSerializer.Serialize(result, new JsonSerializerOptions()
                        {
                            IgnoreNullValues = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        return Ok(message);
                    }
                }
            }

            //in all other cases, if left or right or both are empty we skip compare
            return NotFound();
        }


        [HttpPut("{id}/left")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Base64Image), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutLeftAsync([FromRoute] int id, [FromBody] string data)
        {
            if (id == 0)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(data))
                return BadRequest();

            var image = await _imageRepository.GetAsync(id);
            if (image != null)
            {
                image.ContentLeft = data;
                await _imageRepository.SaveChangesAsync();
            }
            else
            {
                image = new Base64Image()
                {
                    Id = id,
                    ContentLeft = data
                };
                _imageRepository.Add(image);
                await _imageRepository.SaveChangesAsync();
            }

            return CreatedAtAction(
                nameof(Get),
                new { id = image.Id },
                image
                );
        }


        [HttpPut("{id}/right")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Base64Image), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutRight([FromRoute] int id, [FromBody] string data)
        {
            if (id == 0)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(data))
                return BadRequest();

            var image = await _imageRepository.GetAsync(id);
            if (image != null)
            {
                image.ContentRight = data;
                await _imageRepository.SaveChangesAsync();
            }
            else
            {
                image = new Base64Image()
                {
                    Id = id,
                    ContentRight = data
                };
                _imageRepository.Add(image);
                await _imageRepository.SaveChangesAsync();
            }

            return CreatedAtAction(
                nameof(Get),
                new { id = image.Id },
                image
            );
        }
    }
}
