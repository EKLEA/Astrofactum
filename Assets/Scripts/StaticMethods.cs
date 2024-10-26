using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMethods
{
	public static Sprite CropTextureToSprite(Texture2D texture, Vector2Int origin, int size )
	{
		if (texture == null)
		{
			Debug.LogError("Texture is null.");
			return null;
		}

		if (origin.x < 0 || origin.y < 0 || origin.x + size > texture.width || origin.y + size > texture.height)
		{
			Debug.LogError("Crop area is out of bounds.");
			return null;
		}

		// Получаем пиксели из указанной области
		Color[] pixels = texture.GetPixels(
			origin.x, 
			origin.y, 
			size, 
			size
			
		);

		// Создаем новую текстуру и заполняем ее пикселями
		Texture2D croppedTexture = new Texture2D(size, size);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply(); // Применяем изменения

		// Создаем спрайт из новой текстуры
		Sprite croppedSprite = Sprite.Create(
			croppedTexture, 
			new Rect(0, 0, croppedTexture.width, croppedTexture.height), 
			new Vector2(0.5f, 0.5f) // Центрирование спрайта
		);

		return croppedSprite;
	}
	public static Texture2D CropTexture(Texture2D texture, Vector2Int origin, int size )
	{
		if (texture == null)
		{
			Debug.LogError("Texture is null.");
			return null;
		}

		if (origin.x < 0 || origin.y < 0 || origin.x + size > texture.width || origin.y + size > texture.height)
		{
			Debug.LogError("Crop area is out of bounds.");
			return null;
		}
		// Получаем пиксели из указанной области
		Color[] pixels = texture.GetPixels(
			origin.x, 
			origin.y, 
			size, 
			size
		);

		// Создаем новую текстуру и заполняем ее пикселями
		Texture2D croppedTexture = new Texture2D(size, size);
		croppedTexture.SetPixels(pixels);
		croppedTexture.Apply(); // Применяем изменения

		return croppedTexture;
	}
	public static int ComparisonPoints(Vector3 p1,Vector3 p2,Vector3 v)
	{
		Vector3 v1=p1-p1;
		Vector3 v2=p2-p1;
		float projV1=Vector3.Dot(v1,v);
		float projV2=Vector3.Dot(v2,v);
		if(projV2>projV1)
			return 2;
		else if(projV2<projV1)
			return 1;
		else 
			return 0;
	}
	public static Vector3 RotatePoint(Vector3 point, Vector3 pivot, float angle, Vector3 axis)
    {
        // Переносим точку к началу координат
        Vector3 direction = point - pivot;

        // Поворачиваем вектор
        Vector3 rotatedDirection = Quaternion.AngleAxis(angle, axis) * direction;

        // Возвращаем вектор обратно
        return rotatedDirection + pivot;
    }
}
