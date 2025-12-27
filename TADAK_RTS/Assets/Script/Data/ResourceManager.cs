using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ResourceManager : MonoBehaviour {
    private static ResourceManager instance; // Lazy Initialization 싱글톤
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new GameObject("ResourceManager").AddComponent<ResourceManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    // 로드된 핸들을 관리하여 나중에 메모리 해제(Release)를 가능하게 함
    private Dictionary<string, AsyncOperationHandle<GameObject>> handles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    public async Task<GameObject> GetBuildingPrefab(string id) {
        // 이미 로드 중이거나 로드 완료된 경우
        if (handles.TryGetValue(id, out var handle)) {
            await handle.Task;
            return handle.Result;
        }

        // 새로운 로드 요청 (Addressables 주소가 ID와 동일하다고 가정)
        var loadHandle = Addressables.LoadAssetAsync<GameObject>(id);
        handles.Add(id, loadHandle);

        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded) {
            return loadHandle.Result;
        } else {
            Debug.LogError($"[ResourceManager] Failed to load Addressable: {id}");
            handles.Remove(id);
            return null;
        }
    }

    // 메모리 관리를 위해 에셋 해제 기능 추가
    public void ReleaseAsset(string id) {
        if (handles.TryGetValue(id, out var handle)) {
            Addressables.Release(handle);
            handles.Remove(id);
        }
    }
}