from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
import umap
import hdbscan

app = FastAPI(title="BindingChaos Clustering Service")


class ClusterRequest(BaseModel):
    signal_ids: list[str]
    embeddings: list[list[float]]


class ClusterResult(BaseModel):
    signal_id: str
    cluster_label: int


class ClusterResponse(BaseModel):
    results: list[ClusterResult]


@app.get("/health")
def health():
    return {"status": "ok"}


@app.post("/cluster", response_model=ClusterResponse)
def cluster(request: ClusterRequest):
    if len(request.signal_ids) != len(request.embeddings):
        raise HTTPException(status_code=400, detail="signal_ids and embeddings must have the same length")

    if len(request.embeddings) < 2:
        return ClusterResponse(results=[
            ClusterResult(signal_id=sid, cluster_label=-1)
            for sid in request.signal_ids
        ])

    matrix = np.array(request.embeddings, dtype=np.float32)

    n_neighbors = min(15, len(matrix) - 1)
    reducer = umap.UMAP(n_neighbors=n_neighbors, n_components=5, metric="cosine", random_state=42)
    reduced = reducer.fit_transform(matrix)

    min_cluster_size = max(2, len(matrix) // 20)
    clusterer = hdbscan.HDBSCAN(min_cluster_size=min_cluster_size, metric="euclidean")
    labels = clusterer.fit_predict(reduced)

    return ClusterResponse(results=[
        ClusterResult(signal_id=sid, cluster_label=int(label))
        for sid, label in zip(request.signal_ids, labels)
    ])
