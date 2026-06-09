package java_proveedor.src.sockets;

import java_proveedor.src.services.CalculoTarifa;
import java_proveedor.src.services.BitacoraService;
import java_proveedor.src.services.ConsultaSaldo;
import java_proveedor.src.services.RegistrarMovimiento;
import java_proveedor.src.services.VerificarSaldo;
import java_proveedor.src.services.AdministracionTelefonica;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

public class ManejoCliente extends Thread
{
    private Socket clienteSocket;

    ConsultaSaldo consultaSaldo = new ConsultaSaldo();
    CalculoTarifa calculoTarifa = new CalculoTarifa();
    RegistrarMovimiento registrarMovimiento = new RegistrarMovimiento();
    VerificarSaldo verificarSaldo = new VerificarSaldo();
    AdministracionTelefonica administracionTelefonica = new AdministracionTelefonica();
    BitacoraService bitacoraService = BitacoraService.obtenerInstancia();

    public ManejoCliente(Socket clienteSocket)
    {
        this.clienteSocket = clienteSocket;
    }

    @Override
    public void run()
    {
        try (
            BufferedReader reader = new BufferedReader(
                new InputStreamReader(clienteSocket.getInputStream(), StandardCharsets.UTF_8)
            );
            PrintWriter writer = new PrintWriter(
                new OutputStreamWriter(clienteSocket.getOutputStream(), StandardCharsets.UTF_8),
                true
            )
        )
        {
            String solicitud = reader.readLine();
            bitacoraService.registrarEntrada(solicitud);

            if (solicitud == null || solicitud.trim().isEmpty())
            {
                enviarRespuesta(writer, "{\"status\":\"ERROR\",\"mensaje\":\"Trama vacia\"}");
                return;
            }

            String accion = leerCampo(solicitud, "accion");

            if (accion.isEmpty())
            {
                enviarRespuesta(writer, "{\"status\":\"ERROR\",\"mensaje\":\"Accion no encontrada\"}");
                return;
            }

            switch (accion)
            {
                case "VERIFICAR_SALDO":
                case "INICIAR_LLAMADA":
                    procesarVerificacion(solicitud, writer);
                    break;

                case "FINALIZAR_LLAMADA":
                case "REGISTRO_MOVIMIENTO":
                case "REBAJAR_SALDO":
                    procesarRegistroMovimiento(solicitud, writer);
                    break;

                case "CONSULTAR_SALDO":
                    procesarConsultaSaldo(solicitud, writer);
                    break;

                case "DETALLE_TELEFONO":
                    procesarDetalleTelefono(solicitud, writer);
                    break;

                case "RECARGAR_SALDO":
                    procesarRecargaSaldo(solicitud, writer);
                    break;

                case "REGISTRAR_TELEFONO":
                    procesarRegistroTelefono(solicitud, writer);
                    break;

                case "CAMBIAR_ESTADO_TELEFONO":
                    procesarCambioEstadoTelefono(solicitud, writer);
                    break;

                default:
                    enviarRespuesta(writer, "{\"status\":\"ERROR\",\"mensaje\":\"Accion no reconocida: " + accion + "\"}");
                    break;
            }
        }
        catch (Exception e)
        {
            bitacoraService.registrarError(e.getMessage());
            e.printStackTrace();
        }
        finally
        {
            try
            {
                if (clienteSocket != null && !clienteSocket.isClosed())
                {
                    clienteSocket.close();
                }
            }
            catch (IOException ex)
            {
                ex.printStackTrace();
            }
        }
    }

    private void procesarVerificacion(String solicitud, PrintWriter writer)
    {
        String origen = leerCampo(solicitud, "origen");
        if (origen.isEmpty())
        {
            origen = leerCampo(solicitud, "telefono_origen");
        }

        String tipoDestino = leerCampo(solicitud, "tipo_destino");
        if (tipoDestino.isEmpty())
        {
            tipoDestino = leerCampo(solicitud, "tipo_llamada");
        }
        if (tipoDestino.isEmpty())
        {
            tipoDestino = "NACIONAL";
        }

        String respuesta = verificarSaldo.procesarVerificacionLlamada(origen, tipoDestino);
        enviarRespuesta(writer, respuesta);
    }

    private void procesarConsultaSaldo(String solicitud, PrintWriter writer)
    {
        String numero = leerCampo(solicitud, "numero");
        if (numero.isEmpty())
        {
            numero = leerCampo(solicitud, "telefono_origen");
        }

        String resultadoSaldo = consultaSaldo.procesarConsulta(numero);

        if ("ERROR".equals(resultadoSaldo))
        {
            enviarRespuesta(writer, respuestaConsultaSaldoError(consultaSaldo.getUltimoError()));
        }
        else
        {
            enviarRespuesta(writer, respuestaConsultaSaldoOk(numero, resultadoSaldo));
        }
    }

    private void procesarRegistroMovimiento(String solicitud, PrintWriter writer)
    {
        String respuesta = registrarMovimiento.procesarRegistroMovimiento(solicitud);
        enviarRespuesta(writer, respuesta);
    }

    private void procesarDetalleTelefono(String solicitud, PrintWriter writer)
    {
        String numero = leerCampo(solicitud, "telefono");
        if (numero.isEmpty())
        {
            numero = leerCampo(solicitud, "telefono_origen");
        }

        enviarRespuesta(writer, administracionTelefonica.procesarDetalleTelefono(numero));
    }

    private void procesarRecargaSaldo(String solicitud, PrintWriter writer)
    {
        String numero = leerCampo(solicitud, "telefono");
        if (numero.isEmpty())
        {
            numero = leerCampo(solicitud, "telefono_origen");
        }

        String monto = leerCampo(solicitud, "monto");
        enviarRespuesta(writer, administracionTelefonica.procesarRecarga(numero, monto));
    }

    private void procesarRegistroTelefono(String solicitud, PrintWriter writer)
    {
        String numero = leerCampo(solicitud, "telefono");
        String tipoServicio = leerCampo(solicitud, "tipo_servicio");
        String proveedorCodigo = leerCampo(solicitud, "proveedor_codigo");
        String saldoInicial = leerCampo(solicitud, "saldo_inicial");
        String activoTexto = leerCampo(solicitud, "activo");
        boolean activo = !"false".equalsIgnoreCase(activoTexto) && !"0".equals(activoTexto);

        enviarRespuesta(
            writer,
            administracionTelefonica.procesarRegistro(
                numero,
                tipoServicio,
                proveedorCodigo,
                saldoInicial,
                activo
            )
        );
    }

    private void procesarCambioEstadoTelefono(String solicitud, PrintWriter writer)
    {
        String numero = leerCampo(solicitud, "telefono");
        if (numero.isEmpty())
        {
            numero = leerCampo(solicitud, "telefono_origen");
        }

        String activoTexto = leerCampo(solicitud, "activo");
        boolean activo = !"false".equalsIgnoreCase(activoTexto) && !"0".equals(activoTexto);

        enviarRespuesta(writer, administracionTelefonica.procesarCambioEstado(numero, activo));
    }

    private void enviarRespuesta(PrintWriter writer, String respuesta)
    {
        bitacoraService.registrarSalida(respuesta);
        writer.println(respuesta);
    }

    private String respuestaConsultaSaldoOk(String numero, String saldo)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"CONSULTAR_SALDO\","
            + "\"telefono_origen\":\"" + sanitizar(numero) + "\","
            + "\"status\":\"OK\","
            + "\"estado\":\"OK\","
            + "\"saldo\":\"" + sanitizar(saldo) + "\","
            + "\"moneda\":\"CRC\","
            + "\"resultado\":{"
                + "\"codigo\":\"OK\","
                + "\"estado\":\"CONSULTA_EXITOSA\","
                + "\"mensaje\":\"Saldo consultado correctamente\""
            + "},"
            + "\"datos_autorizacion\":{"
                + "\"saldo_disponible\":\"" + sanitizar(saldo) + "\","
                + "\"moneda\":\"CRC\""
            + "}"
        + "}";
    }

    private String respuestaConsultaSaldoError(String mensaje)
    {
        return "{"
            + "\"tipo_transaccion\":\"RESPUESTA_PROVEEDOR\","
            + "\"accion\":\"CONSULTAR_SALDO\","
            + "\"status\":\"ERROR\","
            + "\"estado\":\"ERROR\","
            + "\"mensaje\":\"" + sanitizar(mensaje) + "\","
            + "\"resultado\":{"
                + "\"codigo\":\"ERROR\","
                + "\"estado\":\"CONSULTA_FALLIDA\","
                + "\"mensaje\":\"" + sanitizar(mensaje) + "\""
            + "}"
        + "}";
    }

    public String leerCampo(String solicitud, String campo)
    {
        try
        {
            int posClave = solicitud.indexOf("\"" + campo + "\"");

            if (posClave == -1)
            {
                return "";
            }

            int posDosPuntos = solicitud.indexOf(":", posClave);

            if (posDosPuntos == -1)
            {
                return "";
            }

            int inicio = posDosPuntos + 1;

            while (inicio < solicitud.length() && Character.isWhitespace(solicitud.charAt(inicio)))
            {
                inicio++;
            }

            boolean texto = inicio < solicitud.length() && solicitud.charAt(inicio) == '"';

            if (texto)
            {
                inicio++;
            }

            int fin = inicio;

            while (fin < solicitud.length())
            {
                char actual = solicitud.charAt(fin);

                if (texto && actual == '"')
                {
                    break;
                }

                if (!texto && (actual == ',' || actual == '}' || Character.isWhitespace(actual)))
                {
                    break;
                }

                fin++;
            }

            return solicitud.substring(inicio, fin).trim();
        }
        catch (Exception e)
        {
            return "";
        }
    }

    private String sanitizar(String texto)
    {
        if (texto == null || texto.isBlank())
        {
            return "Error no especificado";
        }

        return texto.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
