# app/services/bitacora.py
import queue
import time
import json

# Cola global donde los hilos de los clientes depositarán las tramas (Entrada y Salida)
cola_bitacora = queue.Queue()

def iniciar_consumidor_bitacora():
    """
    Consumidor que corre en un hilo independiente. 
    Saca elementos de la cola uno por uno y los escribe de forma ordenada.
    """
    print("[Bitácora] Consumidor asíncrono listo y escuchando la cola...")
    while True:
        try:
            registro = cola_bitacora.get()
            if registro is None:  # Señal de parada
                break
            
            # Formato exacto exigido: DD/MM/YYYY: {"telefono": ...}
            fecha_actual = time.strftime("%d/%m/%Y")
            linea = f"{fecha_actual}: {json.dumps(registro, ensure_ascii=False)}\n"
            
            # Escritura segura en el archivo de texto plano
            with open("bitacora_identificador.txt", "a", encoding="utf-8") as archivo:
                archivo.write(linea)
                
            cola_bitacora.task_done()
        except Exception as e:
            print(f"[Bitácora] Error crítico al escribir en el archivo: {e}")