namespace SIGEMAV.Areas.Maquinaria.Funciones
{
    public class BaseResponse
    {
        public bool Exito { get; set; }

        public string Mensaje { get; set; } = string.Empty;


        // =====================================================
        // RESPUESTA EXITOSA
        // =====================================================

        public static BaseResponse Success(
            string mensaje = "Operación realizada correctamente.")
        {
            return new BaseResponse
            {
                Exito = true,
                Mensaje = mensaje
            };
        }


        // =====================================================
        // RESPUESTA CON ERROR DE NEGOCIO
        // =====================================================

        public static BaseResponse Error(
            string mensaje)
        {
            return new BaseResponse
            {
                Exito = false,
                Mensaje = mensaje
            };
        }
    }
}
