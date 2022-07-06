using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private AbstractMap _map;

	/// <summary>
	/// The rate at which the transform's position tries catch up to the provided location.
	/// </summary>
	[SerializeField]
	float _positionFollowFactor;

	/// <summary>
	/// The rate at which the transform's rotation tries catch up to the provided heading.  
	/// </summary>
	[SerializeField]
	[Tooltip("The rate at which the transform's rotation tries catch up to the provided heading. ")]
	float _rotationFollowFactor = 1;

	[SerializeField]
	private GameObject CameraPivot;

	[SerializeField]
	private GameObject Character;

	[SerializeField]
	private Animator animator;

	/// <summary>
	/// Use a mock <see cref="T:Mapbox.Unity.Location.TransformLocationProvider"/>,
	/// rather than a <see cref="T:Mapbox.Unity.Location.EditorLocationProvider"/>. 
	/// </summary>
	[SerializeField]
	bool _useTransformLocationProvider;

	bool _isInitialized;

	private bool[] touchDidMove = new bool[10];

	/// <summary>
	/// The location provider.
	/// This is public so you change which concrete <see cref="T:Mapbox.Unity.Location.ILocationProvider"/> to use at runtime.
	/// </summary>
	ILocationProvider _locationProvider;
	public ILocationProvider LocationProvider
	{
		private get
		{
			if (_locationProvider == null)
			{
				_locationProvider = _useTransformLocationProvider ?
					LocationProviderFactory.Instance.TransformLocationProvider : LocationProviderFactory.Instance.DefaultLocationProvider;
			}

			return _locationProvider;
		}
		set
		{
			if (_locationProvider != null)
			{
				_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;

			}
			_locationProvider = value;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}
	}

	Vector3 _targetPosition;

	void Start()
	{
		LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		_map.OnInitialized += () => _isInitialized = true;
	}

	void OnDestroy()
	{
		if (LocationProvider != null)
		{
			LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
		}
	}

	void LocationProvider_OnLocationUpdated(Location location)
	{
		if (_isInitialized && location.IsLocationUpdated)
		{
			_targetPosition = _map.GeoToWorldPosition(location.LatitudeLongitude);
		}
	}

	void Update()
	{
		Vector3 direction = _targetPosition - transform.position;
		direction.y = 0;
		if (direction != Vector3.zero)
		{
			Character.transform.rotation = Quaternion.Lerp(Character.transform.rotation, Quaternion.LookRotation(direction, transform.up), Time.deltaTime * _rotationFollowFactor);
		}
		transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * _positionFollowFactor);

		animator.SetFloat("Speed", Vector2.Distance(transform.position, _targetPosition) / 200f);

		foreach (var touch in Input.touches)
        {
			if (touch.phase == TouchPhase.Began)
            {
				touchDidMove[touch.fingerId] = false;
            }
			else if (touch.phase == TouchPhase.Moved)
            {
				touchDidMove[touch.fingerId] = true;
				CameraPivot.transform.Rotate(0, 0.05f * touch.deltaPosition.x, 0);
			}
			else if (touch.phase == TouchPhase.Ended && !touchDidMove[touch.fingerId])
            {
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))
                {
					DungeonPortal portal = hit.transform.gameObject.GetComponent<DungeonPortal>();
					if (portal != null)
                    {
						portal.ShowInfo();
                    }
                }
            }
        }
	}
}
