from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import numpy as np
import umap
import hdbscan
from sklearn.feature_extraction.text import TfidfVectorizer

app = FastAPI(title="BindingChaos Clustering Service")


class ClusterRequest(BaseModel):
    signal_ids: list[str]
    embeddings: list[list[float]]
    signal_texts: list[str]


class ClusterResult(BaseModel):
    signal_id: str
    cluster_label: int
    keywords: list[str]


class ClusterResponse(BaseModel):
    results: list[ClusterResult]


@app.get("/health")
def health():
    return {"status": "ok"}


@app.post("/cluster", response_model=ClusterResponse)
def cluster(request: ClusterRequest):
    if len(request.signal_ids) != len(request.embeddings):
        raise HTTPException(status_code=400, detail="signal_ids and embeddings must have the same length")

    if len(request.signal_ids) != len(request.signal_texts):
        raise HTTPException(status_code=400, detail="signal_ids and signal_texts must have the same length")

    if len(request.embeddings) < 2:
        return ClusterResponse(results=[
            ClusterResult(signal_id=sid, cluster_label=-1, keywords=[])
            for sid in request.signal_ids
        ])

    matrix = np.array(request.embeddings, dtype=np.float32)

    n_neighbors = min(15, len(matrix) - 1)
    reducer = umap.UMAP(n_neighbors=n_neighbors, n_components=5, metric="cosine", random_state=42)
    reduced = reducer.fit_transform(matrix)

    min_cluster_size = max(2, len(matrix) // 20)
    clusterer = hdbscan.HDBSCAN(min_cluster_size=min_cluster_size, metric="euclidean")
    labels = clusterer.fit_predict(reduced)

    label_to_texts: dict[int, list[str]] = {}
    for text, label in zip(request.signal_texts, labels):
        lbl = int(label)
        if lbl == -1:
            continue
        label_to_texts.setdefault(lbl, []).append(text)

    cluster_keywords: dict[int, list[str]] = {}
    if label_to_texts:
        all_labels = sorted(label_to_texts.keys())
        corpus = [" ".join(label_to_texts[lbl]) for lbl in all_labels]
        vectorizer = TfidfVectorizer(stop_words="english")
        tfidf_matrix = vectorizer.fit_transform(corpus)
        feature_names = vectorizer.get_feature_names_out()
        for i, lbl in enumerate(all_labels):
            row = tfidf_matrix[i].toarray()[0]
            top_indices = row.argsort()[-5:][::-1]
            cluster_keywords[lbl] = [feature_names[j] for j in top_indices if row[j] > 0]

    return ClusterResponse(results=[
        ClusterResult(
            signal_id=sid,
            cluster_label=int(label),
            keywords=cluster_keywords.get(int(label), []),
        )
        for sid, label in zip(request.signal_ids, labels)
    ])
