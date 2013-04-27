using BGGAPI.Interface;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.Common
{
    public static class IocInitializer
    {
        public static void Initialize()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IBGGApiClient, BGGAPI.BGGAPIClient>();
        }
    }
}
