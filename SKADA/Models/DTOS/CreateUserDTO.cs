﻿namespace SKADA.Models.DTOS
{
    public class CreateUserDTO
    {
        public string Name { get; set; }

        public string Surname { get; set; }


        public string Email { get; set; }

        public string Password { get; set; }

        List<String> analogInputsIds { get; set; }

        List<String> digitalInputsIds { get; set; }
    }
}
