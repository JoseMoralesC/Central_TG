package java_proveedor.src.sockets;

import java_proveedor.src.services.CalculoTarifa;
import java_proveedor.src.services.ConsultaSaldo;
import java_proveedor.src.services.RegistrarMovimiento;
import java_proveedor.src.services.VerificarSaldo;

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

            if (solicitud == null || solicitud.trim().isEmpty())
            {
                writer.println("{\"status\":\"ERROR\",\"mensaje\":\"Trama vacia\"}");
                return;
            }

            String accion = leerCampo(solicitud, "accion");

            if (accion.isEmpty())
            {
                writer.println("{\"status\":\"ERROR\",\"mensaje\":\"Accion no encontrada\"}");
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
                    writer.println("{\"status\":\"OK\",\"mensaje\":\"Movimiento recibido por proveedor\"}");
                    break;

                case "CONSULTAR_SALDO":
                    procesarConsultaSaldo(solicitud, writer);
                    break;

                default:
                    writer.println("{\"status\":\"ERROR\",\"mensaje\":\"Accion no reconocida: " + accion + "\"}");
                    break;
            }
        }
        catch (Exception e)
        {
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
        writer.println(respuesta);
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
            writer.println("{\"status\":\"ERROR\",\"mensaje\":\"" + sanitizar(consultaSaldo.getUltimoError()) + "\"}");
        }
        else
        {
            writer.println("{\"status\":\"OK\",\"saldo\":\"" + resultadoSaldo + "\",\"moneda\":\"CRC\"}");
        }
    }

    public String leerCampo(String solicitud, String campo)
    {
        try
        {
            int posClave = solicitud.indexOf("\"" + campo + "\":");

            if (posClave == -1)
            {
                return "";
            }

            int posDosPuntos = solicitud.indexOf(":", posClave);
            int posAbreComillas = solicitud.indexOf("\"", posDosPuntos);
            int posCierraComillas = solicitud.indexOf("\"", posAbreComillas + 1);

            if (posAbreComillas == -1 || posCierraComillas == -1)
            {
                return "";
            }

            return solicitud.substring(posAbreComillas + 1, posCierraComillas);
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
