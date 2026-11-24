using AutoMapper;
using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Proyecto_Estadias.Mapping
{
    public class WebProfile:Profile
    {
        public WebProfile()
        {
            CreateMap<Proyecto_Estadias.ViewModels.UsuariosVM, Proyecto_Estadias.Models.Usuarios>()
                //.ForMember(dest => dest.Roles, origen => origen.MapFrom(c => c.RolVM))
                //.ForMember(dest => dest.Empresas, origen => origen.MapFrom(c => c.empresaVM))
                .ForMember(dest => dest.UsuariosEmpresas, origen => origen.MapFrom(c => c.UsuariosRolesVMs)).ReverseMap();


            CreateMap<Proyecto_Estadias.ViewModels.EmpleadosVM, Proyecto_Estadias.Models.Empleados>()
                .ForMember(dest => dest.Departamentos, origen => origen.MapFrom(c => c.DepatamentosVM))
               .ForMember(dest => dest.SubEmpresas, origen => origen.MapFrom(c => c.SubEmpresaVM)).ReverseMap();

   
            
        CreateMap<Proyecto_Estadias.Models.Permisos, Proyecto_Estadias.ViewModels.PermisosVM>()
       .ForMember(dest => dest.Empleados, opt => opt.Ignore())
       .ForMember(dest => dest.SubEmpresas, opt => opt.Ignore())
       .ForMember(dest => dest.MotivosAusencia, opt => opt.Ignore())
       .ReverseMap();

            CreateMap<Proyecto_Estadias.Models.Permisos, Proyecto_Estadias.ViewModels.PermisosPdfVM>()
   .ForMember(dest => dest.Empleados, opt => opt.MapFrom(c => c.Empleados))
   .ForMember(dest => dest.SubEmpresas,opt => opt.MapFrom(c => c.SubEmpresas))
   .ForMember(dest => dest.MotivosAusencia, opt => opt.MapFrom(c => c.MotivosAusencia))
   .ReverseMap();


            CreateMap<Proyecto_Estadias.ViewModels.MotivosAusenciaVM, MotivosAusencia>().ReverseMap();
            CreateMap<Proyecto_Estadias.ViewModels.EmpresaVM, Empresas>().ReverseMap();
        
            CreateMap<Proyecto_Estadias.ViewModels.SubEmpresasVM, SubEmpresas>().ReverseMap();
            CreateMap<Proyecto_Estadias.ViewModels.UsuariosRolVM, UsuariosEmpresas>().ReverseMap();
            CreateMap<Proyecto_Estadias.ViewModels.DepartamentosVM, Departamentos>().ReverseMap();
        }
            public static void Run()
        {
            AutoMapper.Mapper.Initialize(a =>
            {
                a.AddProfile<WebProfile>();
            });
        }
    }
}