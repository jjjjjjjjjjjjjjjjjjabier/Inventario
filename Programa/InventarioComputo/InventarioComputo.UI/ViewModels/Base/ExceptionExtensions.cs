using System;

namespace InventarioComputo.UI.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Obtiene un mensaje de error amigable incluyendo detalles del InnerException
        /// </summary>
        public static string GetFriendlyMessage(this Exception ex)
        {
            var baseEx = ex.GetBaseException();
            
            // Si tiene un InnerException con un mensaje �til, mostrarlo
            if (baseEx != ex && !string.IsNullOrWhiteSpace(baseEx.Message))
            {
                return baseEx.Message;
            }

            // Para excepciones EF espec�ficas, extraer mensaje m�s �til
            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException?.Message.Contains("duplicate key") == true ||
                    dbUpdateEx.InnerException?.Message.Contains("UNIQUE KEY") == true)
                {
                    return "Ya existe un registro con los mismos valores en campos que deben ser �nicos.";
                }
                
                if (dbUpdateEx.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return "No se puede realizar la operaci�n porque este registro est� siendo usado por otros datos.";
                }
            }

            return ex.Message;
        }
    }
}