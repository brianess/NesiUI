// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using System;
using System.Numerics;

namespace Nesi.UI.Utils;

public static class AppMath
{
    /// <summary>
    /// Converte uma posição de dimensão 3D para coordenadas 2D da tela do monitor.
    /// </summary>
    /// <param name="matrix">Array de 16 floats representando a View Matrix (dwViewMatrix).</param>
    /// <param name="worldPos">Posição 3D do jogador ou entidade.</param>
    /// <param name="screenPos">Coordenadas 2D resultantes na tela.</param>
    /// <param name="screenWidth">Largura da resolução do seu jogo/tela.</param>
    /// <param name="screenHeight">Altura da resolução do seu jogo/tela.</param>
    /// <returns>Retorna verdadeiro se a entidade estiver na frente da câmera e visível na tela.</returns>
    public static bool WorldToScreen(float[] matrix, Vector3 worldPos, out Vector2 screenPos, int screenWidth, int screenHeight)
    {
        screenPos = new Vector2(0, 0);

        // Se a matriz não for válida, evita crash
        if (matrix == null || matrix.Length < 16)
            return false;

        // Cálculo do componente W (Clip Space) para checar se está atrás da câmera
        float w = matrix[12] * worldPos.X + matrix[13] * worldPos.Y + matrix[14] * worldPos.Z + matrix[15];

        // Se W for menor que 0.01, o objeto está atrás do jogador
        if (w < 0.01f)
            return false;

        // Multiplicação da matriz para os eixos X e Y
        float x = matrix[0] * worldPos.X + matrix[1] * worldPos.Y + matrix[2] * worldPos.Z + matrix[3];
        float y = matrix[4] * worldPos.X + matrix[5] * worldPos.Y + matrix[6] * worldPos.Z + matrix[7];

        // Divisão de perspectiva (Transforma para Normalized Device Coordinates - NDC)
        float invW = 1.0f / w;
        x *= invW;
        y *= invW;

        // Conversão de NDC (-1 a 1) para pixels da tela (Screen Space)
        float centerX = screenWidth / 2.0f;
        float centerY = screenHeight / 2.0f;

        screenPos.X = centerX + (centerX * x);
        screenPos.Y = centerY - (centerY * y); // Invertido porque no espaço de tela o Y cresce para baixo

        return true;
    }

    public static Vector2 GetCenter(Vector2 size, Vector2 baseSize)
    {
        return new Vector2((baseSize.X / 2) - (size.X / 2), (baseSize.Y / 2) - (size.Y / 2));
    }

    public static float GetCenterX(float width, float baseWidth)
    {
        return (baseWidth / 2) - (width / 2);
    }

    public static float GetCenterY(float height, float baseHeight)
    {
        return (baseHeight / 2) - (height / 2);
    }
}


