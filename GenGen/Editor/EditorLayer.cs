using GenGen.ECS;
using GenGen.ECS.Components;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace GenGen.Editor;

/// <summary>
/// Редактор поверх игры. Hierarchy + Inspector.
/// </summary>
public class EditorLayer(World world)
{
	private Entity _selected = Entity.None;

	/// <summary>
	/// Вызывать между BeforeLayout и AfterLayout.
	/// </summary>
	public void Draw()
	{
		DrawHierarchy();
		DrawInspector();
	}

	private void DrawHierarchy()
	{
		ImGui.Begin("Hierarchy");

		foreach (var (entity, _) in world.Query<TransformComponent>())
		{
			var label    = $"Entity {entity.Id}";
			var selected = entity == _selected;

			if (ImGui.Selectable(label, selected))
				_selected = entity;
		}

		ImGui.End();
	}

	private void DrawInspector()
	{
		ImGui.Begin("Inspector");

		if (_selected == Entity.None)
		{
			ImGui.TextDisabled("Ничего не выбрано");
			ImGui.End();
			return;
		}

		ImGui.Text($"Entity {_selected.Id}");
		ImGui.Separator();

		if (world.Get<TransformComponent>(_selected) is { } transform)
		{
			if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen))
			{
				var pos = new System.Numerics.Vector2(transform.Position.X, transform.Position.Y);
				if (ImGui.DragFloat2("Position", ref pos, 1f))
					transform.Position = new Vector2(pos.X, pos.Y);

				var rot = transform.Rotation;
				if (ImGui.DragFloat("Rotation", ref rot, 0.01f))
					transform.Rotation = rot;
			}
		}

		if (world.Get<VelocityComponent>(_selected) is { } velocity)
		{
			if (ImGui.CollapsingHeader("Velocity", ImGuiTreeNodeFlags.DefaultOpen))
			{
				var vel = new System.Numerics.Vector2(velocity.Velocity.X, velocity.Velocity.Y);
				if (ImGui.DragFloat2("Velocity", ref vel, 1f))
					velocity.Velocity = new Vector2(vel.X, vel.Y);
			}
		}

		if (world.Get<SpriteStackComponent>(_selected) is { } sprite)
		{
			if (ImGui.CollapsingHeader("SpriteStack", ImGuiTreeNodeFlags.DefaultOpen))
			{
				var scale = sprite.Scale;
				if (ImGui.DragFloat("Scale", ref scale, 0.1f, 0.1f, 20f))
					sprite.Scale = scale;

				ImGui.LabelText("Layers", sprite.Layers.Length.ToString());
			}
		}

		ImGui.End();
	}
}
