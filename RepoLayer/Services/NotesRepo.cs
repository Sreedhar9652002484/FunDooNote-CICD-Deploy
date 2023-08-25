using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommonLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Services
{
    public class NotesRepo: INotesRepo
    {
        
        private readonly FunDoContext funDoContext;
        private readonly IConfiguration configuration;
        private readonly Cloudinary _cloudinary;
        private readonly FileService _fileService;

        public NotesRepo(FunDoContext funDoContext, IConfiguration configuration, Cloudinary cloudinary, FileService fileService)
        {
            this.funDoContext = funDoContext;
            this.configuration = configuration;
            this._cloudinary = cloudinary;
            _fileService = fileService;
        }


        public NotesEntity NoteTaking(NoteTakingModel model, long userId)
        {

            try
            {
                //var userId = funDoContext.User.FirstOrDefault(u => u.UserId == userId);

                //(userId != null)

                NotesEntity notesEntity = new NotesEntity();
                notesEntity.Title = model.Title;
                notesEntity.TakeNote = model.TakeaNote;
                notesEntity.UserId = userId;
                /* notesEntity.Remainder = model.Remainder;
                 notesEntity.Colour = model.Colour;
                 notesEntity.Image = model.Image;
                 notesEntity.isArchive = model.isArchive;
                 notesEntity.isPin = model.isPin;
                 notesEntity.isTrash = model.isTrash;
                 notesEntity.UserId = model.UserId;*/

                funDoContext.Notes.Add(notesEntity);
                funDoContext.SaveChanges();

                if (notesEntity != null)
                {
                    return notesEntity;
                }
                return null;

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GenerateJwtToken(string Email, long UserId)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                 new Claim("UserId", UserId.ToString()),
                // Add any other claims you want to include in the token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(configuration["JwtSettings:Issuer"], configuration["JwtSettings:Audience"], claims, DateTime.Now, DateTime.Now.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public List<NotesEntity> GetAllNotes(long userId)
        {
            try
            {
                return funDoContext.Notes.Where(data => data.UserId == userId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<NotesEntity> GetNotesById(long NotesId)
        {
            try
            {

                var notesById=funDoContext.Notes.FirstOrDefault(u=>u.NoteId == NotesId);
                if (notesById != null)
                {
                    List<NotesEntity> newList = new List<NotesEntity>();
                    newList.Add(notesById); 
                    return newList;
                }
                return null;
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string UpdateNote(long NotesId, string Notes, long userId)
        {
            try
            {
                var userNotes = funDoContext.Notes.FirstOrDefault(data => data.NoteId == NotesId&&data.UserId==userId);
                //var userIdNote = funDoContext.User.FirstOrDefault(data => data.UserId == userId);
                if (userNotes != null)
                {

                    
                    userNotes.TakeNote = userNotes.TakeNote +" "+ Notes;
                    funDoContext.Notes.Update(userNotes);
                    funDoContext.SaveChanges();

                    return userNotes.TakeNote;

                }
                return null;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool DeleteNote(long NotesId, long userId)
        {
            try
            {
                var note = funDoContext.Notes.FirstOrDefault(data => data.NoteId == NotesId&&data.UserId==userId);
                if (note != null)
                {
                    
                    funDoContext.Notes.Remove(note);
                    funDoContext.SaveChanges();
                    return true;

                }
                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }

       public string Colour(long NotesId, string colour, long userId)
        {
            try
            {
                var note = funDoContext.Notes.FirstOrDefault(data => data.NoteId == NotesId && data.UserId == userId);
                if (note != null)
                {
                   note.Colour = colour; 
                   funDoContext.Notes.Update(note);
                    funDoContext.SaveChanges() ;
                    return note.Colour;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Tuple<int, string>> AddImage(long NotesId, long userId, string item, IFormFile imageFile)
        {
            try
            {
                var note = funDoContext.Notes.FirstOrDefault(data => data.NoteId == NotesId && data.UserId == userId);
                if (note != null)
                {
                    var fileServiceResult = await _fileService.SaveImage(imageFile);

                    if (fileServiceResult.Item1 == 0)
                    {
                        return new Tuple<int, string>(0, fileServiceResult.Item2);
                    }
                        // Upload image to Cloudinary
                        var uploadimg = new ImageUploadParams
                        {
                            File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                        };
                        ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uploadimg);

                        // ... the rest of your code ...
                        // Update product entity with image URL from Cloudinary
                        string ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
                        // Add the product entity to the DbContext
                            note.Image = ImageUrl;
                            funDoContext.Notes.Update(note);
                            funDoContext.SaveChanges();
                   
                        return new Tuple<int, string>(1, "Product added with image successfully");
                    }
                return null;
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "An error occurred: " + ex.Message);
            }
            
        }
       public bool Archive(long NotesId, long userId)

        {
            try
            {
                var notes = funDoContext.Notes.FirstOrDefault(u => u.NoteId == NotesId && u.UserId == userId);
                if(notes != null)
                {
                    notes.isArchive = true;
                  
                    funDoContext.Notes.Update(notes);
                    funDoContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Pin(long NotesId, long userId)
        {
            try
            {
                var notes = funDoContext.Notes.FirstOrDefault(u => u.NoteId == NotesId && u.UserId == userId);
                if (notes != null)
                {
                    notes.isPin=true;
                    funDoContext.Notes.Update(notes);
                    funDoContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool MoveToTrash(long NotesId, long userId)
        {
            try
            {
                var notes = funDoContext.Notes.FirstOrDefault(u => u.NoteId == NotesId && u.UserId == userId);
                if (notes != null)
                {
                    notes.isTrash = true;
                    funDoContext.Notes.Update(notes);
                    funDoContext.SaveChanges();


                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        // tasks is to develop an API that enables users to find notes based on keywords or phrases. 
    //    The API should accept a search query parameter and return the search results

        public List<NotesEntity> FindNotes(string keyword, long userId) 
        {
            try
            {
                var result = funDoContext.Notes.FirstOrDefault(u => u.UserId == userId);
                if (result != null)
                {
                    var findNotes = funDoContext.Notes.Where(u => u.UserId == userId && u.TakeNote.Contains(keyword)).ToList();
                  if (findNotes.Count > 0)
                    return findNotes;

                }
                return null;

            }
            catch (Exception)
            {

                throw;
            }
        
        }
        public List<NotesEntity> CopyNotes(long userId, long noteId)
        {
            try
            {
                var result = funDoContext.Notes.FirstOrDefault(u => u.UserId == userId);
                if (result != null)
                {
                    var noteCopy = funDoContext.Notes.FirstOrDefault(n => n.NoteId == noteId);
                    if (noteCopy != null)
                    {
                        var newNote = new NotesEntity
                        {
                            //NoteId = newNotedId,
                            UserId = userId,
                            Title = noteCopy.Title,
                            TakeNote = noteCopy.TakeNote,
                            Reminder = noteCopy.Reminder,
                            Colour = noteCopy.Colour,
                            Image = noteCopy.Image,
                            isArchive = noteCopy.isArchive,
                            isPin = noteCopy.isPin,
                            isTrash = noteCopy.isTrash
                        };
                        funDoContext.Notes.Add(newNote);
                        funDoContext.SaveChanges();
                        List<NotesEntity> list = new List<NotesEntity>();
                        list.Add(newNote);
                       return list;

                    }
                    return null;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
